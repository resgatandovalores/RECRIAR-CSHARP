# Run with dot-sourcing in the same PowerShell session used by dotnet/Visual Studio:
# . ./docs/database/Set-LocalConnection.ps1
# The value exists only in this process environment; no configuration is saved.
$localConnection = [System.Data.Common.DbConnectionStringBuilder]::new()
$localConnection['Server'] = '127.0.0.1'
$localConnection['Port'] = Read-Host 'Porta do MySQL local (normalmente 3306)'
$localConnection['Database'] = 'recriar_dev'
$localConnection['User ID'] = Read-Host 'Usuario do MySQL'
$localPassword = Read-Host 'Senha do MySQL' -AsSecureString
$passwordPointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($localPassword)
try {
    $localConnection['Password'] = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($passwordPointer)
    $env:RECRIAR_CONNECTION_STRING = $localConnection.ConnectionString
}
finally {
    [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($passwordPointer)
    $localConnection.Clear()
    $localPassword.Dispose()
}
Write-Host 'Conexao configurada apenas nesta sessao para recriar_dev no servidor local.'
