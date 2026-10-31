# Banco de dados do RECRIAR

Preparei o banco com 11 tabelas, os relacionamentos e os nove tipos de parentesco. Também fiz as classes que salvam, consultam, editam e excluem os cadastros, seguindo as interfaces que já estavam no projeto.

Incluí o registro de atividades do sistema e as tabelas de programas, oficinas, encontros, inscrições e presenças. Essa parte está preparada no banco, mas ainda não tem telas.

Os arquivos incluem o SQL com dados fictícios, os diagramas e as instruções para conectar às telas. As classes de Domain, Application e os formulários existentes foram mantidos.

O projeto compilou sem erros nem avisos. Passaram 26 verificações no MySQL, incluindo edição de familiares, exclusão dos dados vinculados e falhas no registro de atividades. As telas e a geração do HTML real não foram testadas nessas verificações.

Falta conectar o serviço às telas no Program.cs, na parte do Pedro.

Destino do PR: **sandbox**.
