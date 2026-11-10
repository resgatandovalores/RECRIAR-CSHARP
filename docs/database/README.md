# Banco de dados do RECRIAR

Esta versão usa SQL direto com ADO.NET, seguindo o modelo de acesso ao banco mostrado na foto da aula. As operações usam MySqlConnection, MySqlCommand, parâmetros e os métodos ExecuteReader, ExecuteNonQuery e ExecuteScalar. Não é necessário usar Entity Framework, Configurations ou migrations nesta versão.

## Os dois bancos

| Banco | Para que serve |
|---|---|
| recriar_prod | Guarda os dados nas 11 tabelas. Recebe inclusões, alterações e exclusões. |
| recriar_leitura | Possui 11 views para consultar os dados atuais do banco principal. |

Uma view é uma consulta salva que aparece como uma tabela para quem consulta. Ela não guarda outra cópia dos dados. Quando o cadastro muda no banco principal, a próxima consulta no banco de leitura já vê a mudança. As views foram criadas com ALGORITHM=TEMPTABLE, que impede inclusão, edição e exclusão por elas.

**Decisão para esta entrega:** a mensagem do grupo pediu dois bancos, mas a foto não explica como devem se relacionar. Foi adotada uma separação por dois schemas no mesmo servidor, com views para leitura. Isso não é uma réplica independente nem um segundo servidor. Se o professor pediu replicação ou uma cópia física, essa parte precisa ser ajustada após confirmar a orientação.

O nome recriar_prod identifica o banco principal do projeto. O pacote é para instalação e apresentação acadêmica no computador local; não representa uma implantação em um servidor da ONG.

## Como instalar no MySQL

1. Entre no MySQL Workbench com uma conta administradora.
2. Abra `database.sql` e execute o arquivo completo. Ele cria os dois bancos novos, as 11 tabelas, as 11 views, os parentescos e os papéis de acesso. Não apaga bancos existentes e não inclui crianças fictícias.
3. Se recriar_prod ou recriar_leitura já existir, pare e confira o que há nesses bancos antes de executar o instalador. Comandos de estrutura não são desfeitos por ROLLBACK no MySQL.
4. Para mostrar exemplos, execute `sample-data.sql` somente em uma instalação local destinada a testes.
5. Execute as consultas de `queries.sql`. A demonstração de edição e exclusão fica em uma transação desfeita com ROLLBACK.

Os mesmos passos de criação estão separados em `dois-bancos/01_criar_prod.sql`, `02_criar_leitura.sql` e `03_permissoes.sql`. Escolha o instalador completo ou os arquivos separados; não execute ambos na mesma instalação.

## Contas para gravação e consulta

Crie duas contas locais novas no Workbench, escolhendo as senhas ali. Por exemplo: recriar_app e recriar_reader, com host localhost. Elas não devem ter permissões globais nem outros papéis. Depois de executar o instalador, atribua:

```sql
GRANT 'recriar_gravacao' TO 'recriar_app'@'localhost';
SET DEFAULT ROLE 'recriar_gravacao' TO 'recriar_app'@'localhost';
GRANT 'recriar_consulta' TO 'recriar_reader'@'localhost';
SET DEFAULT ROLE 'recriar_consulta' TO 'recriar_reader'@'localhost';
```

A conta de gravação recebe SELECT, INSERT, UPDATE e DELETE apenas em recriar_prod. A conta de consulta recebe apenas SELECT em recriar_leitura. Os nomes acima são exemplos: ajuste se criar contas com outros nomes. As senhas não ficam nos scripts.

As views usam a permissão da conta que as criou para ler o banco principal. Mantenha essa conta existente e com permissão de leitura em recriar_prod. Se recriar as views com outra conta, confira novamente as permissões.

## Como configurar o programa

É necessário .NET 10, MySQL 8.0.16 ou mais recente e o pacote MySqlConnector 2.6.2, já indicado em Infrastructure.

Na pasta que contém Recriar.sln, execute:

```powershell
dotnet restore
. ./docs/database/Set-LocalConnection.ps1
dotnet build Recriar.sln
```

O script pede a porta e os dados das duas contas. Ele configura RECRIAR_CONNECTION_STRING para gravação e RECRIAR_READ_CONNECTION_STRING para consulta somente na sessão atual. Inicie o programa nessa mesma sessão para que receba essas configurações. Não copie os valores das variáveis para o GitHub.

## Integração com as telas

As interfaces e as classes de Domain e Application foram mantidas. Os nomes das classes usadas pelo Pedro também foram preservados:

```csharp
using Recriar.Application.Interfaces;
using Recriar.Application.Services;
using Recriar.Infrastructure.Persistence.Context;
using Recriar.Infrastructure.Persistence.Repositories;
using Recriar.Infrastructure.Logging;
using Recriar.Infrastructure.Export;

using var context = AppDbContextFactory.Create();
IChildService service = new ChildService(
    new ChildRepository(context),
    new RelationshipTypeRepository(context),
    new DbActivityLogger(),
    new FileErrorLogger(),
    new HtmlChildFileExporter());
```

As telas do PR #9 foram integradas ao MySQL em Program.cs. CreateService agora cria os componentes reais e entrega o ChildService à MainForm. O código de abertura confere a conexão de gravação e a carga de parentescos da leitura, preservando o FileErrorLogger e System.Windows.Forms.Application por extenso. O desenho dos formulários foi mantido.

AppDbContext agora é uma classe simples que guarda as configurações das duas conexões. O nome foi mantido por compatibilidade; ela não herda de DbContext e não usa o Entity Framework. Cada método abre e fecha a conexão de que precisa. A leitura vai para recriar_leitura e a gravação para recriar_prod.

GetById busca a criança, o responsável, o contato e os familiares com seus parentescos. Na edição, comandos UPDATE alteram os registros existentes, INSERT inclui familiares novos e DELETE remove os que saíram da ficha. Os IDs dos familiares mantidos não mudam.

A gravação do cadastro usa uma transação: se falhar alguma parte, o conjunto inteiro é desfeito. Os IDs são passados de volta ao objeto somente depois da confirmação da transação. O histórico usa outra conexão, pois uma falha no registro da atividade não deve desfazer o cadastro.

## Regras preservadas

- As 11 tabelas são Children, Guardians, EmergencyContacts, FamilyMembers, RelationshipTypes, ActivityLogs, Programs, Activities, ActivitySessions, SessionEnrollments e AttendanceRecords.
- Cada criança tem no máximo um registro de responsável e um de contato. O serviço exige os dois ao cadastrar. Uma inserção direta por SQL ainda pode omitir esses registros dependentes.
- O catálogo tem nove parentescos; Outro é sempre Id 9. A descrição de Outro e o limite de dez familiares são conferidos pelo serviço.
- CPF, RG e telefone são texto. CPF pode se repetir. As idades aceitas vão de 0 a 120, conforme o código original.
- Os textos não aceitam NULL, exceto OtherRelationship. Endereço, escola, centro de saúde, ficha e alergia podem ficar vazios. Os tamanhos das colunas continuam iguais aos limites do serviço.
- Excluir a criança remove responsável, contato, familiares, inscrições e presenças. O histórico permanece. Parentesco em uso não pode ser apagado.
- Uma criança não pode ter duas inscrições no mesmo encontro nem duas presenças na mesma inscrição. O fim do encontro deve ocorrer depois do início. Programas, atividades e encontros com vínculos têm a exclusão bloqueada.
- Os horários de registro usam UTC. Os encontros também devem seguir essa convenção; a tela pode converter para o horário local.

As tabelas de oficinas estão preparadas e testadas no banco. Os serviços e as telas de oficinas ainda precisam ser feitos. O modelo não implementa proteção contra duas pessoas editando simultaneamente o mesmo cadastro.

## Testes e diagramas

O projeto compilou com zero erros e zero avisos. Passaram 42 verificações em MySQL real com ADO.NET, incluindo transações, edição de familiares, consultas, bloqueio de gravação na leitura, permissões das contas e exclusão em cascata.

Para repetir, configure RECRIAR_TEST_CONNECTION_STRING para um administrador de um MySQL local de testes e execute:

```powershell
dotnet run --project tests/Recriar.Persistence.IntegrationTests
```

Os testes criam dois bancos e duas contas com nomes aleatórios e removem somente esses objetos ao terminar. O usuário precisa ter permissão para isso. Eles não alteram recriar_prod nem recriar_leitura.

O programa de testes verifica o HTML real, inclusive seu conteúdo, além de usar um exportador de teste em outros cenários. A abertura, a pesquisa, a ficha de edição e o aviso de conexão indisponível também foram conferidos no Windows. O checklist completo de salvar, editar familiares, reabrir, exportar e excluir pelos botões ainda exige uma rodada manual; veja RELATORIO_INTEGRACAO.md. MER.png mostra o modelo conceitual. EER.png e RECRIAR.mwb mostram as 11 tabelas do principal. O desenho Dois_Bancos.svg explica a ligação entre os bancos.

O guia COMO_ENTENDER_O_CODIGO.md ajuda a estudar o acesso ao banco. ENVIO_GITHUB.md explica o envio da branch para revisão. O destino do PR continua sendo sandbox.

## Integração com as telas publicada pelo grupo

As telas vieram do [PR #9](https://github.com/resgatandovalores/RECRIAR-CSHARP/pull/9), já incorporado à sandbox no commit fdcc5c0. A branch local desta entrega é integration/mysql-winforms. O arquivo RELATO_BANCO.md explica a parte feita em linguagem simples, sem sugestão de slides.

## Referências

- Foto da aula enviada pelo grupo e classes da branch sandbox, atualizada com o PR #9 no commit fdcc5c0.
- [API básica do MySqlConnector](https://mysqlconnector.net/tutorials/basic-api/).
- [Views de leitura no MySQL](https://dev.mysql.com/doc/refman/8.0/en/view-algorithms.html).
