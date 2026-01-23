Get-Content .env | ForEach-Object {
    $key, $value = $_.Split('=', 2); 
    [System.Environment]::SetEnvironmentVariable($key.Trim(), $value.Trim())
}
