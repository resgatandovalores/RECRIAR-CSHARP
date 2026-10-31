# Banco de dados do RECRIAR

Esta entrega implementa a persistência dos cadastros da branch `sandbox`. O serviço e as classes existentes foram preservados. Os rascunhos anteriores de três tabelas devem ser substituídos por este modelo.

## Estrutura

São 11 tabelas de negócio: `Children`, `Guardians`, `EmergencyContacts`, `FamilyMembers`, `RelationshipTypes`, `ActivityLogs`, `Programs`, `Activities`, `ActivitySessions`, `SessionEnrollments` e `AttendanceRecords`. Há também a tabela técnica `__EFMigrationsHistory`, criada pelo EF Core, que não entra nessa contagem.

O cadastro está integrado aos contratos existentes. As cinco tabelas de oficinas são uma extensão aprovada para esta entrega: possuem entidades, mapeamentos, migration, restrições, dados de exemplo e testes. Ainda não possuem telas nem novos serviços de negócio. A inscrição é por encontro, o que permite registrar uma presença sem relacioná-la acidentalmente a outro encontro.

## Regras preservadas

- Cada criança pode ter no máximo um registro de responsável e um contato de emergência. O serviço exige esses dados no cadastro. As FKs e os índices únicos impedem duplicidade, mas não obrigam, sozinhos, que toda criança inserida diretamente por SQL tenha ambos os dependentes.
- Excluir a criança remove responsável, contato, familiares, inscrições e presenças. Os logs permanecem com a descrição da operação; não possuem FK para a criança.
- Parentesco em uso não pode ser excluído. Os nove IDs são fixos, com `RelationshipType.OtherId = 9` para Outro.
- CPF, RG e telefone são texto. CPF não é único. Idades de criança e familiar variam de 0 a 120, conforme o código original.
- Todos os textos são NOT NULL, exceto `OtherRelationship`. Endereço, escola, centro de saúde, número da ficha e alergia podem ser strings vazias.
- Os limites de tamanho usam as constantes `FieldLimits`. Dez familiares por criança e preenchimento de Outro são regras do serviço; não foram adicionados triggers que mudem esse contrato.
- Um programa possui atividades; uma atividade possui encontros; uma criança pode se inscrever em vários encontros. A combinação encontro/criança é única. Cada inscrição admite zero ou um registro de presença. Sem registro significa ainda não avaliado; `Attended = false` significa falta.
- Programa com atividades, atividade com encontros e encontro com inscrições não podem ser excluídos sem tratar os vínculos. A exclusão da criança continua tendo prioridade sobre os dados de participação dela.

## Dependências

SDK .NET 10, Pomelo 9.0.0, EF Core e dotnet-ef 9.0.22, MySqlConnector 2.6.2. Os pacotes foram instalados somente em Infrastructure. O uso de EF Core 9 em net10.0 evita misturar a versão do EF com a versão suportada pelo provedor.

O manifesto `dotnet-tools.json` fixa a versão de `dotnet-ef`; não é necessário instalar a ferramenta globalmente.

## Preparar o banco local

1. Na raiz do repositório, execute `dotnet tool restore` e `dotnet restore`.
2. Configure `RECRIAR_CONNECTION_STRING` apenas no ambiente da sessão. O script `. ./docs/database/Set-LocalConnection.ps1` pede porta, usuário e senha sem gravar um arquivo. Não publique o valor da variável.
3. Confira servidor, porta e nome do banco antes de executar migrations. A fábrica aceita somente loopback e `recriar_dev`.
4. Execute `dotnet ef database update --project src/Recriar.Infrastructure --startup-project src/Recriar.Infrastructure`.
5. Para dados fictícios, selecione `recriar_dev` no Workbench e execute `sample-data.sql`. Não rode esse arquivo em uma base com dados reais.

Como alternativa às migrations, `database.sql` cria um schema novo com estrutura, seed, dados fictícios e consultas. **Escolha um caminho de criação.** Se o schema já existir, use migrations ou escolha outro banco de demonstração no SQL. O arquivo não contém DROP DATABASE. MySQL faz commits implícitos em comandos DDL, portanto uma instalação interrompida pode deixar estrutura parcial.

O SQL criado pela migration também registra seu histórico. Após instalar por SQL, `database update` reconhece a migration aplicada. `schema.sql` contém apenas a estrutura e o seed de parentescos; `queries.sql` reúne as consultas por tela. A pesquisa por trecho não usa o índice de nome como uma busca por prefixo; esse índice não elimina o custo de `%texto%`.

## Integração com as telas do Pedro

Namespaces:

```csharp
using Recriar.Application.Interfaces;
using Recriar.Application.Services;
using Recriar.Infrastructure.Persistence.Context;
using Recriar.Infrastructure.Persistence.Repositories;
using Recriar.Infrastructure.Logging;
using Recriar.Infrastructure.Export;
```

Montagem sugerida, preservando os nomes do guia:

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

`Program.cs` e os formulários não foram alterados. O contexto deve ser compartilhado pelos dois repositórios e usado de forma síncrona, em uma única thread. `GetById` retorna o agregado rastreado; o serviço modifica esse objeto e chama `Update`. Remover um familiar da lista exclui o órfão; adicionar um familiar com Id zero insere uma linha. O repositório não aceita Update de um agregado desconectado.

O log usa um contexto independente a cada chamada e grava horário UTC. A interface deve converter para horário local apenas na exibição. Se SaveChanges falhar em um repositório, seu rastreamento é limpo antes de propagar o erro, para não repetir alterações pendentes no próximo comando. Um contexto de longa duração não detecta automaticamente mudanças feitas por outra instância do programa; esta entrega segue o uso desktop de um usuário previsto no guia, sem controle de concorrência otimista.

## Diagramas

`RECRIAR.mwb` é o modelo editável gerado por engenharia reversa do MySQL local. `EER.png` é a exportação do diagrama. `MER.svg` e `MER.png` apresentam o modelo conceitual. No Windows, o MySQL pode apresentar os nomes das tabelas em minúsculas por causa de `lower_case_table_names`; o SQL e os mapeamentos mantêm os nomes definidos em C#.

Para atualizar o EER depois de mudar o banco, use Database > Reverse Engineer no Workbench, selecione `recriar_dev`, importe as tabelas e organize o diagrama. O histórico de migrations está no catálogo, mas foi omitido da imagem para destacar as 11 tabelas de negócio.

## Testes

`dotnet build Recriar.sln` deve terminar sem avisos nem erros. Os testes de persistência são um executável de integração:

```text
dotnet run --project tests/Recriar.Persistence.IntegrationTests
```

Eles leem `RECRIAR_TEST_CONNECTION_STRING`, aceitam apenas servidor local e criam um schema com nome aleatório `recriar_test_...`. O usuário informado deve poder criar e remover esse schema. O próprio runner remove somente esse schema ao terminar; nunca usa o nome do banco informado na configuração. Os testes não apontam para `recriar_dev` e não precisam das telas.

Foram aprovadas 26 verificações no MySQL 8.0.46, incluindo gravação pelo ChildService, edição de familiares, cascata, restrições, consulta por nome, seed e isolamento do log. O SQL completo também foi executado em um schema separado. A exportação HTML e as telas não foram validadas nesse runner: o exportador é substituído por um stub para testar a interação do serviço com o log.

## Git

A implementação foi preparada em `feature/leandro-database`, a partir de `sandbox`. O arquivo `PR.md` contém uma descrição pronta para revisão. O destino do PR deve ser `sandbox`. Nenhuma credencial ou configuração local deve entrar no commit.

## Referências

- Guia leandro-bd.md e classes da branch sandbox, commit base `f6347f0`.
- [Provedor Pomelo](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql).
- [Chaves estrangeiras no MySQL](https://dev.mysql.com/doc/refman/8.4/en/create-table-foreign-keys.html).
- [Execução de scripts no Workbench](https://dev.mysql.com/doc/workbench/en/wb-command-line-options.html).
