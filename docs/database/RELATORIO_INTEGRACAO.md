# Integração do Windows Forms com o MySQL

## Base usada

- Telas do [PR #9](https://github.com/resgatandovalores/RECRIAR-CSHARP/pull/9), incorporado à sandbox no commit fdcc5c0.
- Implementação ADO.NET local da parte do banco, commit 393adaa.
- Orientações do arquivo leandro-integracao-mysql.md fornecido pelo grupo.
- Branch de integração: integration/mysql-winforms.

## O que mudou

As alterações de integração estão em `src/Recriar.WinForms/Program.cs`. Os formulários do Pedro foram mantidos como vieram no PR.

CreateService deixou de devolver null. Ele usa AppDbContextFactory para obter a configuração e cria ChildRepository, RelationshipTypeRepository e DbActivityLogger e usa a fábrica CreateChildService já prevista pelas telas. MainForm recebe o serviço real. O exportador HtmlChildFileExporter e o FileErrorLogger foram preservados.

Antes de abrir a tela, a inicialização tenta abrir a conexão de gravação e carrega a lista de parentescos pela conexão de leitura. Ela verifica Outro no Id 9. Se houver falha, mostra uma mensagem com as verificações de configuração e registra o erro em arquivo. A mensagem não exibe a senha nem o valor completo das conexões.

AppDbContext é uma classe simples de configuração, sem uma conexão aberta permanente. Os repositórios e o logger abrem e descartam uma conexão por operação. Não há rastreamento de objetos do Entity Framework para limpar entre duas edições.

Os testes foram ampliados para conferir leitura em uma nova instância e exportação com o HtmlChildFileExporter real. Os documentos foram atualizados para retirar a indicação antiga de que a ligação com as telas ainda não existia.

## Configuração local

1. Instalar os bancos pelo `docs/database/database.sql`, com uma conta administradora, em um MySQL local de testes. Se os bancos já existirem, conferir a instalação antes de executar novamente.
2. Criar as duas contas locais e atribuir os papéis conforme o README. O principal tem 11 tabelas; a leitura tem 11 views. Outro deve ter Id 9.
3. Na raiz do projeto, executar `dotnet restore`.
4. Na mesma sessão que iniciará o programa, executar `. ./docs/database/Set-LocalConnection.ps1` e informar porta e credenciais das duas contas.
5. Executar `dotnet run --project src/Recriar.WinForms` nessa sessão.

RECRIAR_CONNECTION_STRING aponta para recriar_prod. RECRIAR_READ_CONNECTION_STRING aponta para recriar_leitura, no mesmo servidor e porta. Senhas não devem ser adicionadas ao código, ao Git ou a mensagens de erro.

Esta solução usa dois schemas no mesmo servidor. As views mostram os dados atuais e bloqueiam gravação. Se o professor exigir uma réplica física, essa parte ainda precisa ser revista.

## Resultado das verificações

| Verificação | Resultado e limite |
|---|---|
| Compilar Recriar.sln | Aprovado, zero erros e zero avisos. |
| Criar estrutura e carregar parentescos | Aprovado em MySQL local; Outro no Id 9. |
| Iniciar o Windows Forms com MySQL | Aprovado; tela principal listou uma ficha fictícia do banco. |
| Pesquisar por parte do nome e abrir edição | Aprovado pela interface; a ficha foi carregada do MySQL. |
| Salvar ficha com dois familiares, incluindo Outro | Aprovado pelo ChildService e MySQL. O preenchimento completo de um novo cadastro pela tela ainda não foi concluído. |
| Reabrir e buscar os dados | Aprovado com uma nova instância do acesso ao banco. Fechar e reabrir a aplicação completa após salvar pela interface ainda precisa ser exercitado. |
| Alterar, remover e incluir familiares na mesma edição | Aprovado pelo serviço, preservando o Id do familiar mantido. Salvar essas mudanças pelos botões da interface ainda está pendente. |
| Exportar HTML real | Aprovado pelo serviço; arquivo e conteúdo conferidos com dados do MySQL. O botão e a abertura no navegador não foram conferidos nesta rodada. |
| Excluir e conferir cascata e histórico | Aprovado pelo serviço e consultas no MySQL. O botão de confirmação da interface não foi exercitado. |
| Iniciar com conexão indisponível | Aprovado pela interface; foi exibida uma orientação para conferir servidor, bancos e variáveis de ambiente. |

Foram aprovadas 42 verificações automáticas no MySQL 8.0.46. Os testes usam bancos e contas temporários com dados fictícios. Não foi usado servidor da ONG ou do trabalho.

A automação dos controles do diálogo de edição não ficou confiável neste ambiente. Por isso, a abertura da ficha foi registrada como conferida, mas a conclusão das operações pelos botões não foi marcada como aprovada. Isso não altera o resultado dos testes do serviço.

## O que conferir manualmente antes da apresentação

Em uma instalação local de testes, abrir Novo cadastro e salvar uma ficha fictícia com dois familiares, incluindo Outro. Fechar e reabrir o aplicativo, pesquisar o nome e abrir a ficha. Na mesma edição, alterar um familiar, remover o outro e incluir um novo. Salvar e reabrir para conferir. Depois, exportar pelo botão e conferir o HTML no navegador. Por último, excluir a ficha pela interface e conferir os dados vinculados e o histórico.

Essa rodada completa pela interface continua pendente. As telas e os serviços específicos de oficinas também não fazem parte desta integração.

## Entrega e Git

As alterações estão em uma branch local de integração. Não houve push nem abertura de PR nesta etapa. O destino previsto é sandbox. A permissão de envio da conta do usuário precisa estar resolvida antes de publicar.
