# Integração das telas do RECRIAR ao MySQL

As telas incorporadas pelo PR #9 ainda iniciavam sem serviço de banco, pois CreateService retornava null. Esta entrega reúne essas telas com a persistência ADO.NET e monta os componentes reais no Program.cs. A MainForm passa a receber um ChildService conectado aos dois bancos locais.

A abertura verifica a conexão de gravação e o parentesco Outro com Id 9 na leitura. Em caso de falha, mantém o log em arquivo e mostra uma orientação de configuração. As regras de Domain e Application foram preservadas.

Inclui os scripts de recriar_prod e recriar_leitura, os repositórios com SQL parametrizado, transações e permissões de consulta. O banco de leitura usa views no mesmo servidor, não uma réplica independente.

Validação: solution com zero erros e zero avisos; 60 verificações aprovadas em MySQL, incluindo exportação HTML real e nova instância de leitura. Em 09/11/2026, corrigi a identificação das janelas de cadastro e familiar, ativando ShowInTaskbar nos dois formulários. Antes, a ferramenta tentava preencher os controles pela janela principal bloqueada. Depois do ajuste, consegui selecionar cada janela e preencher os campos. Isso resolveu o impedimento do teste automatizado da interface.

Pela interface, cadastrei a ficha fictícia 2 com dois familiares, incluindo Outro/Primo, pesquisei e reabri a ficha. Na mesma edição, alterei um familiar, removi outro e incluí um novo. Salvei, fechei e reiniciei o aplicativo. O MySQL confirmou idade 9, o familiar mantido no Id 2 e o novo no Id 4. O botão Exportar ficha gerou o HTML com os dados atualizados. O Windows mostrou o seletor de aplicativo para abrir HTML; a visualização no navegador não foi concluída. Por fim, excluí a ficha pelo botão e confirmei zero registros da criança, responsável, contato e familiares, com o histórico preservado.

Base: sandbox com o PR #9, commit fdcc5c0. Destino: sandbox. Branch local: integration/mysql-winforms.
