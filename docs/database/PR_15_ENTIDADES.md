## Ampliação do banco para 15 entidades

O mínimo informado para o trabalho passou a ser 15 entidades. Esta atualização acrescenta Educators (educadores), Volunteers (voluntários), Units (unidades) e Materials (materiais), com relacionamentos às oficinas, programas e encontros.

- Banco principal com 15 tabelas e banco de leitura com 15 views.
- Script atualizar_11_para_15.sql para atualizar a instalação anterior sem apagar os cadastros.
- Campo UnitId opcional nos encontros para manter os dados antigos.
- Exemplos, consultas, documentação, MER, EER e modelo do Workbench atualizados.
- 60 verificações aprovadas no MySQL, incluindo preservação dos dados durante a atualização.

As quatro entidades novas estão implementadas no banco. As telas e os serviços delas no C# ainda não foram desenvolvidos.

Para uma instalação nova, usar database.sql. Para atualizar a instalação com 11 tabelas, fazer backup e executar atualizar_11_para_15.sql uma vez.

Destino: sandbox.
