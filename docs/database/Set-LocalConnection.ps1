# Run in the same PowerShell session used to start the application:
# . ./docs/database/Set-LocalConnection.ps1
# Passwords stay only in this process environment.
do {
    [string] $mysqlPort = Read-Host 'Porta do MySQL local (Enter para 3306)'
    if ([string]::IsNullOrWhiteSpace($mysqlPort)) { $mysqlPort = '3306' }
    [int] $portNumber = 0
    $validPort = [int]::TryParse($mysqlPort, [ref] $portNumber) -and $portNumber -ge 1 -and $portNumber -le 65535
    if (-not $validPort) { Write-Host 'Digite somente a porta, por exemplo: 3306.' }
} until ($validPort)

function Read-RecriarConnection {
    param([string] $Database, [string] $Label)

    Write-Host $Label
    $connectionData = [System.Data.Common.DbConnectionStringBuilder]::new()
    # Add com valores string evita o erro de PSObject do indexador no PowerShell 5.1.
    $connectionData.Add('Server', '127.0.0.1')
    $connectionData.Add('Port', [string] $portNumber)
    $connectionData.Add('Database', $Database)
    do {
        [string] $mysqlUser = Read-Host 'Usuario do MySQL'
        if ([string]::IsNullOrWhiteSpace($mysqlUser)) { Write-Host 'Informe o usuario dessa conta.' }
    } while ([string]::IsNullOrWhiteSpace($mysqlUser))
    $connectionData.Add('User ID', $mysqlUser)
    $securePassword = Read-Host 'Senha do MySQL' -AsSecureString
    $passwordPointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
    try {
        $connectionData.Add('Password', [Runtime.InteropServices.Marshal]::PtrToStringBSTR($passwordPointer))
        return $connectionData.ConnectionString
    }
    finally {
        [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($passwordPointer)
        $connectionData.Clear()
        $securePassword.Dispose()
    }
}

try {
    $writeConnection = Read-RecriarConnection -Database 'recriar_prod' -Label 'Conta de gravacao'
    $readConnection = Read-RecriarConnection -Database 'recriar_leitura' -Label 'Conta de consulta'
    $env:RECRIAR_CONNECTION_STRING = $writeConnection
    $env:RECRIAR_READ_CONNECTION_STRING = $readConnection
}
finally {
    Remove-Variable writeConnection, readConnection -ErrorAction SilentlyContinue
}
Write-Host 'As duas conexoes foram configuradas somente nesta sessao.'
