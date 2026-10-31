# Como enviar ao GitHub e abrir o PR

## O que é um PR

PR significa Pull Request. É um pedido para o grupo revisar suas alterações e juntá-las ao projeto.

Uma branch é uma versão separada do projeto, onde você trabalha na sua parte. Um commit salva um conjunto de alterações no histórico. O push envia os commits do computador ao GitHub. Depois do push, você abre o PR. Quando o grupo aprova e faz o merge, as mudanças entram na branch de destino.

Neste projeto: sua branch → PR → revisão do grupo → sandbox.

Abrir o PR não muda automaticamente a sandbox.

## Na cópia já preparada neste computador

O trabalho está na branch feature/leandro-database e já tem commits locais. Dentro da pasta do repositório, confira:

```powershell
git branch --show-current
git status
git log -2 --oneline
```

Depois de revisar, o comando de envio seria:

```powershell
git push -u origin feature/leandro-database
```

É necessário ter permissão de escrita no repositório. Se o GitHub pedir para entrar na conta, use o procedimento normal de login. Não coloque senha ou token nos arquivos do projeto.

## Se você começar em outra pasta ou computador

Extraia o pacote da entrega. Clone o projeto em uma pasta nova e crie sua branch:

```powershell
git clone -b sandbox https://github.com/resgatandovalores/RECRIAR-CSHARP.git
cd RECRIAR-CSHARP
git switch -c feature/leandro-database
```

O arquivo alteracoes-banco.patch contém somente as mudanças da parte do banco. Troque o caminho do exemplo pelo local onde você extraiu o arquivo:

```powershell
git apply --check "C:/caminho/alteracoes-banco.patch"
git apply "C:/caminho/alteracoes-banco.patch"
dotnet build Recriar.sln
git status
```

Se o primeiro comando apontar conflito, confira os arquivos antes de continuar. Algum colega pode ter alterado a mesma parte. Use o patch para aproveitar suas mudanças sem copiar o projeto inteiro por cima do trabalho dos outros.

Confira a lista de arquivos. Depois salve e envie:

```powershell
git add .gitignore Recriar.sln dotnet-tools.json src/Recriar.Infrastructure docs/database tests
git commit -m "Adiciona o banco de dados do RECRIAR"
git push -u origin feature/leandro-database
```

Se o Git pedir nome e e-mail para o commit, configure os dados que você usa na sua conta. Se faltar permissão para enviar, peça ao responsável para adicionar você como colaborador ou use o fluxo de fork do GitHub.

## Abrir o PR pelo site

1. Abra o repositório RECRIAR-CSHARP no GitHub.
2. Entre em Pull requests e clique em New pull request. Também pode aparecer Compare & pull request depois do envio.
3. Em **base**, selecione **sandbox**. Esse é o destino.
4. Em **compare**, selecione **feature/leandro-database**. Essa é sua branch.
5. Confira os arquivos da comparação.
6. Use o título “Banco de dados do RECRIAR” e copie o texto de PR.md para a descrição.
7. Clique em Create pull request. Depois, compartilhe o link com o grupo para revisão.

Após a revisão, o grupo faz a junção das alterações, chamada merge. Novos commits enviados à mesma branch atualizam o mesmo PR.

Este guia explica como fazer o envio. Os arquivos preparados localmente ainda não foram publicados no GitHub.

Referência: [Como criar um PR no GitHub](https://docs.github.com/en/pull-requests/how-tos/create-pull-requests/creating-a-pull-request).
