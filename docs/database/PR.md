# Banco de dados com ADO.NET e acesso de leitura

O acesso ao MySQL passa a usar SQL direto com MySqlCommand, parâmetros, ExecuteReader, ExecuteNonQuery e ExecuteScalar, conforme o exemplo da aula. As interfaces de cadastro e os nomes usados para montar o serviço foram mantidos.

O instalador cria recriar_prod com 11 tabelas e recriar_leitura com 11 views de consulta. As views mostram os dados atuais do principal e bloqueiam alterações. Foram incluídos papéis separados para gravação e consulta. Essa solução usa dois schemas no mesmo servidor; não é uma réplica física.

Cadastro e edição usam transações. A edição preserva os IDs dos familiares mantidos, inclui os novos e remove os que saíram. O registro de atividades usa outra conexão.

Inclui SQL de instalação, dados fictícios separados, diagramas e guia de conexão com duas contas. Domain, Application e os formulários existentes foram preservados. As configurações e migrations do EF foram substituídas pelos scripts SQL.

Validação: compilação sem erros nem avisos e 39 verificações aprovadas no MySQL, incluindo permissões de leitura e rollback do cadastro e da edição. As telas e a geração do HTML real não foram validadas nesses testes. As oficinas estão modeladas no banco, ainda sem telas e serviços próprios.

Falta conectar o ChildService às telas no Program.cs. O destino do PR é sandbox.
