# Como entender a parte do banco

O código usa SQL direto, no estilo mostrado na foto da aula. Comece por ChildRepository.cs, nos métodos GetAll, Add e Delete.

## A conexão

MySqlConnection representa uma conexão ao MySQL. Open abre a conexão. O bloco using fecha e libera a conexão quando a operação termina, mesmo se ocorrer um erro.

AppDbContext guarda as configurações de gravação e leitura. CreateWriteConnection cria a conexão para recriar_prod; CreateReadConnection cria a conexão para recriar_leitura. Os dados de acesso vêm das variáveis de ambiente, sem senhas no código.

## Os três métodos da aula

| Método | O que faz | Onde aparece |
|---|---|---|
| ExecuteReader | Executa uma consulta e permite ler suas linhas. | GetById, GetAll e consulta de parentescos. |
| ExecuteNonQuery | Executa INSERT, UPDATE ou DELETE e devolve a quantidade de linhas afetadas. | Add, Update, Delete e registro de atividades. |
| ExecuteScalar | Devolve o primeiro valor da primeira linha de uma consulta, ou null se não houver linhas. | Consulta do ID recém-criado e verificação do cadastro. |

ExecuteScalar não serve somente para COUNT: SELECT LAST_INSERT_ID() também devolve um único valor, que é o código gerado na mesma conexão.

## Como os dados entram no comando

```csharp
string sql = "DELETE FROM Children WHERE Id = @id";
using (MySqlCommand command = new MySqlCommand(sql, connection))
{
    command.Parameters.AddWithValue("@id", id);
    int rows = command.ExecuteNonQuery();
}
```

@id é um parâmetro. AddWithValue fornece o valor separado do texto SQL. Isso evita montar SQL juntando o que a pessoa digitou. Os nomes de tabela e coluna do programa são fixos; os valores são passados por parâmetros.

## Como uma consulta vira uma lista

ExecuteReader devolve um leitor. O while (reader.Read()) passa por cada linha encontrada. GetInt32 lê um número inteiro e GetString lê um texto. O código cria um objeto Child, preenche seus campos e o adiciona à lista. ORDER BY Id define a ordem da consulta.

## Como o cadastro é gravado

Add inclui a criança, o responsável, o contato e os familiares. Tudo ocorre na mesma transação. Commit confirma o conjunto de alterações. Se uma parte falhar antes do Commit, o fechamento da transação desfaz o que estava pendente. Assim, não fica uma criança gravada com metade da ficha faltando.

Update altera os campos da criança e dos registros vinculados. Para os familiares, ele consulta os IDs existentes, atualiza os mantidos, inclui os novos e exclui os removidos. Um familiar de outra criança não pode ser atualizado por essa ficha.

Delete usa a quantidade de linhas afetadas para devolver true ou false. O MySQL exclui os dados dependentes pelas chaves estrangeiras com ON DELETE CASCADE.

## O banco de leitura

recriar_leitura contém views, que são consultas salvas. Elas mostram os dados atuais de recriar_prod. As views não permitem gravação e a conta de consulta recebe apenas SELECT. Não existe uma cópia independente para sincronizar nessa solução.

## O que ainda exige um pouco mais de estudo

Transações, chaves estrangeiras e permissões são assuntos que você deve entender para explicar a integridade dos dados. A organização em interfaces vem do projeto do grupo. A parte principal usa métodos, classes, condições, listas, laços e SQL direto. Não há Configurations, migrations ou rastreamento de objetos do Entity Framework nesta versão.
