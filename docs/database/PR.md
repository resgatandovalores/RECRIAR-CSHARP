# Integração das telas do RECRIAR ao MySQL

As telas incorporadas pelo PR #9 ainda iniciavam sem serviço de banco, pois CreateService retornava null. Esta entrega reúne essas telas com a persistência ADO.NET e monta os componentes reais no Program.cs. A MainForm passa a receber um ChildService conectado aos dois bancos locais.

A abertura verifica a conexão de gravação e o parentesco Outro com Id 9 na leitura. Em caso de falha, mantém o log em arquivo e mostra uma orientação de configuração. Os formulários do Pedro e as regras de Domain e Application foram preservados.

Inclui os scripts de recriar_prod e recriar_leitura, os repositórios com SQL parametrizado, transações e permissões de consulta. O banco de leitura usa views no mesmo servidor, não uma réplica independente.

Validação: solution com zero erros e zero avisos; 42 verificações aprovadas em MySQL, incluindo exportação HTML real e nova instância de leitura. No Windows, foram conferidos abertura, pesquisa, abertura de ficha de edição e aviso com conexão indisponível. A rodada completa dos botões de criação, edição de familiares, exportação e exclusão ainda está pendente e está detalhada no relatório.

Base: sandbox com o PR #9, commit fdcc5c0. Destino: sandbox. Branch local: integration/mysql-winforms.
