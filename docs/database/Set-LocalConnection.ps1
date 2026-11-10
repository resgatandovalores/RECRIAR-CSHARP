# Run in the same PowerShell session used to start the application:
# . ./docs/database/Set-LocalConnection.ps1
# Passwords stay only in this process environment.
$mysqlPort = Read-Host 'Porta do MySQL local (normalmente 3306)'

function Read-RecriarConnection {
    param([string] $Database, [string] $Label)

    Write-Host $Label
    $connectionData = [System.Data.Common.DbConnectionStringBuilder]::new()
    $connectionData['Server'] = '127.0.0.1'
    $connectionData['Port'] = $mysqlPort
    $connectionData['Database'] = $Database
    $connectionData['User ID'] = Read-Host 'Usuario do MySQL'
    $securePassword = Read-Host 'Senha do MySQL' -AsSecureString
    $passwordPointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
    try {
        $connectionData['Password'] = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($passwordPointer)
        return $connectionData.ConnectionString
    }
    finally {
        [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($passwordPointer)
        $connectionData.Clear()
        $securePassword.Dispose()
    }
}

$env:RECRIAR_CONNECTION_STRING = Read-RecriarConnection -Database 'recriar_prod' -Label 'Conta de gravacao'
$env:RECRIAR_READ_CONNECTION_STRING = Read-RecriarConnection -Database 'recriar_leitura' -Label 'Conta de consulta'
Write-Host 'As duas conexoes foram configuradas somente nesta sessao.'
