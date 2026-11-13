# Integração do Windows Forms com o MySQL

## Base usada

- Telas do [PR #9](https://github.com/resgatandovalores/RECRIAR-CSHARP/pull/9), incorporado à sandbox no commit fdcc5c0.
- Implementação ADO.NET local da parte do banco, commit 393adaa.
- Orientações do arquivo leandro-integracao-mysql.md fornecido pelo grupo.
- Branch de integração: integration/mysql-winforms.

## O que mudou

As alterações de integração estão em `src/Recriar.WinForms/Program.cs`. Os formulários do Pedro receberam apenas o ajuste de ShowInTaskbar descrito abaixo.

CreateService deixou de devolver null. Ele usa AppDbContextFactory para obter a configuração e cria ChildRepository, RelationshipTypeRepository e DbActivityLogger e usa a fábrica CreateChildService já prevista pelas telas. MainForm recebe o serviço real. O exportador HtmlChildFileExporter e o FileErrorLogger foram preservados.

Antes de abrir a tela, a inicialização tenta abrir a conexão de gravação e carrega a lista de parentescos pela conexão de leitura. Ela verifica Outro no Id 9. Se houver falha, mostra uma mensagem com as verificações de configuração e registra o erro em arquivo. A mensagem não exibe a senha nem o valor completo das conexões.

AppDbContext é uma classe simples de configuração, sem uma conexão aberta permanente. Os repositórios e o logger abrem e descartam uma conexão por operação. Não há rastreamento de objetos do Entity Framework para limpar entre duas edições.

Os testes foram ampliados para conferir leitura em uma nova instância e exportação com o HtmlChildFileExporter real. Os documentos foram atualizados para retirar a indicação antiga de que a ligação com as telas ainda não existia.

## Configuração local

1. Instalar os bancos pelo `docs/database/database.sql`, com uma conta administradora, em um MySQL local de testes. Se os bancos já existirem, conferir a instalação antes de executar novamente.
2. Criar as duas contas locais e atribuir os papéis conforme o README. O principal tem 15 tabelas; a leitura tem 15 views. Outro deve ter Id 9.
3. Na raiz do projeto, executar `dotnet restore`.
4. Na mesma sessão que iniciará o programa, executar `. ./docs/database/Set-LocalConnection.ps1` e informar porta e credenciais das duas contas.
5. Executar `dotnet run --project src/Recriar.WinForms` nessa sessão.

RECRIAR_CONNECTION_STRING aponta para recriar_prod. RECRIAR_READ_CONNECTION_STRING aponta para recriar_leitura, no mesmo servidor e porta. Senhas não devem ser adicionadas ao código, ao Git ou a mensagens de erro.

Esta solução usa dois schemas no mesmo servidor. As views mostram os dados atuais e bloqueiam gravação. Se o professor exigir uma réplica física, essa parte ainda precisa ser revista.

## Resultado das verificações

A solução compilou com zero erros e zero avisos. As 60 verificações automáticas passaram no MySQL 8.0.46 em 13/11/2026, após ampliar o modelo para 15 entidades. O teste da interface descrito abaixo foi feito em 09/11/2026.

Em 09/11/2026, corrigi a identificação das janelas de cadastro e familiar, ativando ShowInTaskbar nos dois formulários. Antes, a ferramenta tentava preencher os controles pela janela principal bloqueada. Depois do ajuste, consegui selecionar cada janela e preencher os campos. Isso resolveu o impedimento do teste automatizado da interface.

Pela interface, cadastrei a ficha fictícia 2 com dois familiares, incluindo Outro/Primo, pesquisei e reabri a ficha. Na mesma edição, alterei um familiar, removi outro e incluí um novo. Salvei, fechei e reiniciei o aplicativo. O MySQL confirmou idade 9, o familiar mantido no Id 2 e o novo no Id 4. O botão Exportar ficha gerou o HTML com os dados atualizados. O Windows mostrou o seletor de aplicativo para abrir HTML; a visualização no navegador não foi concluída. Por fim, excluí a ficha pelo botão e confirmei zero registros da criança, responsável, contato e familiares, com o histórico preservado.

Também foi conferida a mensagem de inicialização com conexão indisponível. Os testes usaram apenas dados fictícios em uma instância local separada na porta 33179. O MySQL habitual não foi alterado.

A geração do HTML pelo botão foi aprovada; a abertura visual no navegador continua sem confirmação. As telas e serviços de oficinas não fazem parte desta integração.

## Entrega e Git

As alterações estão em uma branch local de integração. Não houve push nem abertura de PR nesta etapa. O destino previsto é sandbox. A permissão de envio da conta do usuário precisa estar resolvida antes de publicar.

## Ampliação para 15 entidades em 13/11/2026

Foram acrescentadas Educators, Volunteers, Units e Materials, com chaves estrangeiras, exemplos, consultas e views de leitura. ActivitySessions recebeu UnitId opcional para manter compatibilidade com os encontros antigos.

Os testes cobriram as ligações, quantidade não negativa de materiais, bloqueio de exclusão de unidade em uso, leitura pelas quatro novas views e recusa de gravação pela conta de consulta. Também foi testada a atualização de 11 para 15 com um cadastro já salvo, preservando criança e familiares. O script é atualizar_11_para_15.sql; ele deve ser executado uma vez, após backup.

As telas atuais de cadastro não mudaram nesta ampliação. As quatro entidades novas ainda não têm telas nem serviços no C#.
