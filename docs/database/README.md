# Banco de dados do RECRIAR

Este material contém a parte do banco de dados, feita a partir do código da branch `sandbox`. O serviço e as classes existentes foram mantidos. Este modelo substitui o rascunho antigo de três tabelas.

## Estrutura

O sistema tem 11 tabelas: `Children`, `Guardians`, `EmergencyContacts`, `FamilyMembers`, `RelationshipTypes`, `ActivityLogs`, `Programs`, `Activities`, `ActivitySessions`, `SessionEnrollments` e `AttendanceRecords`. Existe também a tabela `__EFMigrationsHistory`, criada pelo EF Core, que não entra nessa contagem.

As classes que salvam e buscam os cadastros seguem as interfaces que já estavam no projeto. As cinco tabelas de oficinas são uma extensão aprovada para esta entrega: têm classes, configuração das tabelas, regras, exemplos e testes. As telas e as funções para usar as oficinas ainda precisam ser feitas. A inscrição é por encontro, o que permite registrar uma presença sem relacioná-la acidentalmente a outro encontro.

## Regras do banco

- Cada criança pode ter no máximo um registro de responsável e um contato de emergência. O serviço exige esses dados no cadastro. As chaves estrangeiras e os índices únicos impedem registros duplicados, mas não obrigam, sozinhos, que toda criança inserida diretamente por SQL tenha ambos os dependentes.
- Excluir a criança remove responsável, contato, familiares, inscrições e presenças. O histórico das operações permanece salvo, mesmo depois de excluir a criança.
- Parentesco em uso não pode ser excluído. Os nove números de identificação são fixos, com `RelationshipType.OtherId = 9` para Outro.
- CPF, RG e telefone são texto. CPF não é único. Idades de criança e familiar variam de 0 a 120, conforme o código original.
- Todos os textos não aceitam NULL, exceto `OtherRelationship`. Endereço, escola, centro de saúde, número da ficha e alergia podem ficar vazios.
- Os limites de tamanho usam as constantes `FieldLimits`. Dez familiares por criança e preenchimento de Outro são regras do serviço; essa conferência continua sendo feita pelo serviço.
- Um programa possui atividades; uma atividade possui encontros; uma criança pode se inscrever em vários encontros. A combinação encontro/criança é única. Cada inscrição admite zero ou um registro de presença. Sem registro significa ainda não avaliado; `Attended = false` significa falta.
- Programa com atividades, atividade com encontros e encontro com inscrições não podem ser excluídos sem tratar os vínculos. A exclusão da criança continua tendo prioridade sobre os dados de participação dela.

## Ferramentas usadas

SDK .NET 10, Pomelo 9.0.0, EF Core e dotnet-ef 9.0.22, MySqlConnector 2.6.2. Os pacotes foram instalados somente em Infrastructure. O uso de EF Core 9 em net10.0 evita misturar a versão do EF com a versão suportada pelo provedor.

O arquivo `dotnet-tools.json` indica a versão de `dotnet-ef` usada pelo projeto. Execute `dotnet tool restore` para instalar essa ferramenta para o projeto.

## Como preparar o banco no computador

Uma migration é um arquivo que registra uma mudança na estrutura do banco. Os comandos abaixo usam esses arquivos para criar ou atualizar as tabelas.

1. Na raiz do repositório, execute `dotnet tool restore` e `dotnet restore`.
2. Configure `RECRIAR_CONNECTION_STRING` somente na sessão atual. O script `. ./docs/database/Set-LocalConnection.ps1` pede porta, usuário e senha sem gravar um arquivo. Não publique o valor da variável.
3. Confira servidor, porta e nome do banco antes de atualizar as tabelas. A classe que cria a conexão aceita somente o servidor do próprio computador e o banco `recriar_dev`.
4. Execute `dotnet ef database update --project src/Recriar.Infrastructure --startup-project src/Recriar.Infrastructure`.
5. Para dados fictícios, selecione `recriar_dev` no Workbench e execute `sample-data.sql`. Não rode esse arquivo em uma base com dados reais.

Como alternativa às migrations, `database.sql` cria um banco novo com estrutura, seed, dados fictícios e consultas. **Escolha um caminho de criação.** Se o banco já existir, use migrations ou escolha outro banco de demonstração no SQL. O arquivo não contém DROP DATABASE. Se houver uma falha durante a criação por SQL, algumas tabelas podem continuar criadas. O MySQL não desfaz todos os comandos de estrutura com ROLLBACK.

O SQL criado pela migration também registra seu histórico. Após instalar por SQL, `database update` reconhece a migration aplicada. `schema.sql` contém apenas a estrutura e o seed de parentescos; `queries.sql` reúne as consultas por tela. A pesquisa por trecho não usa o índice de nome como uma busca por prefixo; esse índice não elimina o custo de `%texto%`.

## Integração com as telas do Pedro

Importações usadas no exemplo:

```csharp
using Recriar.Application.Interfaces;
using Recriar.Application.Services;
using Recriar.Infrastructure.Persistence.Context;
using Recriar.Infrastructure.Persistence.Repositories;
using Recriar.Infrastructure.Logging;
using Recriar.Infrastructure.Export;
```

Exemplo para criar o serviço que as telas vão usar:

```csharp
using var context = AppDbContextFactory.Create();
IChildService service = new ChildService(
    new ChildRepository(context),
    new RelationshipTypeRepository(context),
    new DbActivityLogger(),
    new FileErrorLogger(),
    new HtmlChildFileExporter());
// Passe service ao construtor da tela que o Pedro implementar.
// Mantenha context vivo durante System.Windows.Forms.Application.Run(...).
```

`Program.cs` e os formulários não foram alterados. O objeto `context` faz a comunicação com o banco. Os dois repositórios precisam usar o mesmo objeto, em uma única thread. `GetById` busca a criança com seus dados vinculados. O serviço altera esse mesmo objeto e chama `Update` para salvar. Se um familiar sair da lista, ele também é excluído do banco. Se entrar um familiar com Id zero, um novo registro é criado. Para editar, use um cadastro buscado pelo mesmo contexto. `Update` não aceita um objeto carregado em outro contexto.

O registro de atividades usa seu próprio contexto a cada chamada e guarda os horários em UTC. A interface deve converter para horário local apenas na exibição. Se SaveChanges falhar em um repositório, seu rastreamento é limpo antes de propagar o erro, para não repetir alterações pendentes no próximo comando. Um contexto de longa duração não detecta automaticamente mudanças feitas por outra instância do programa; esta entrega segue o uso desktop de um usuário previsto no guia, sem uma regra para detectar edições simultâneas de duas pessoas.

## Diagramas

`RECRIAR.mwb` é o modelo editável gerado por engenharia reversa do MySQL local. `EER.png` é a exportação do diagrama. `MER.svg` e `MER.png` apresentam o modelo conceitual. No Windows, o MySQL pode apresentar os nomes das tabelas em minúsculas por causa de `lower_case_table_names`; o SQL e os mapeamentos mantêm os nomes definidos em C#.

Para atualizar o EER depois de mudar o banco, use Database > Reverse Engineer no Workbench, selecione `recriar_dev`, importe as tabelas e organize o diagrama. O histórico de migrations está no catálogo, mas foi omitido da imagem para destacar as 11 tabelas de negócio.

## Testes

`dotnet build Recriar.sln` deve terminar sem avisos nem erros. Para executar os testes do banco:

```text
dotnet run --project tests/Recriar.Persistence.IntegrationTests
```

Eles leem `RECRIAR_TEST_CONNECTION_STRING`, aceitam apenas servidor local e criam um schema com nome aleatório `recriar_test_...`. O usuário informado deve poder criar e remover esse schema. Ao terminar, o programa de testes remove somente o banco temporário que ele criou. Ele não usa o nome de banco informado na configuração. Os testes não apontam para `recriar_dev` e não precisam das telas.

Foram aprovadas 26 verificações no MySQL 8.0.46, incluindo gravação pelo ChildService, edição de familiares, cascata, restrições, consulta por nome, seed e isolamento do log. O SQL completo também foi executado em um schema separado. Esses testes não conferem as telas nem a geração do HTML real. Eles usam um exportador de teste para conferir a chamada do serviço e o registro da operação.

## Git

O trabalho foi preparado na branch `feature/leandro-database`, a partir da `sandbox`. O guia `ENVIO_GITHUB.md` explica o envio e a abertura do PR. O arquivo `PR.md` contém um texto para a descrição. O destino deve ser `sandbox`. Senhas e configurações pessoais ficam fora do envio.

## Referências

- Guia leandro-bd.md e classes da branch sandbox, commit base `f6347f0`.
- [Provedor Pomelo](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql).
- [Chaves estrangeiras no MySQL](https://dev.mysql.com/doc/refman/8.4/en/create-table-foreign-keys.html).
- [Execução de scripts no Workbench](https://dev.mysql.com/doc/workbench/en/wb-command-line-options.html).
