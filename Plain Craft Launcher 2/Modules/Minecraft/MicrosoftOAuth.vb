Imports System
Imports System.Text.RegularExpressions

Public Module MicrosoftOAuth

    Public Const DeviceCodeEndpoint As String = "https://login.microsoftonline.com/consumers/oauth2/v2.0/devicecode"
    Public Const TokenEndpoint As String = "https://login.microsoftonline.com/consumers/oauth2/v2.0/token"
    Public Const Scope As String = "XboxLive.signin offline_access"

    Public Function ResolveClientId(EnvironmentClientId As String, ConfiguredClientId As String, BuildClientId As String) As String
        For Each Value In {EnvironmentClientId, ConfiguredClientId, BuildClientId}
            If Not String.IsNullOrWhiteSpace(Value) Then Return Value.Trim()
        Next
        Return ""
    End Function

    Public Function RequireClientId(ClientId As String) As String
        If String.IsNullOrWhiteSpace(ClientId) Then
            Throw New InvalidOperationException("$尚未配置微软应用 Client ID。请在 设置 → 其他 → 微软登录 中填写此启动器使用的应用（客户端）ID，然后重新登录。")
        End If
        ClientId = ClientId.Trim()
        Dim ApplicationId As Guid
        If Guid.TryParse(ClientId, ApplicationId) Then Return ApplicationId.ToString("D")
        If Regex.IsMatch(ClientId, "\A[0-9a-fA-F]{16}\z") Then Return ClientId
        Throw New InvalidOperationException("$微软应用 Client ID 格式不正确。请复制应用概述中的 应用程序（客户端）ID，不是账户、租户 ID 或应用密钥。")
    End Function

    Public Function CreateDeviceCodeRequest(ClientId As String) As String
        Return "client_id=" & Uri.EscapeDataString(RequireClientId(ClientId)) &
            "&scope=" & Uri.EscapeDataString(Scope)
    End Function

    Public Function CreateDeviceTokenRequest(ClientId As String, DeviceCode As String) As String
        Return "grant_type=urn:ietf:params:oauth:grant-type:device_code" &
            "&client_id=" & Uri.EscapeDataString(RequireClientId(ClientId)) &
            "&device_code=" & Uri.EscapeDataString(DeviceCode)
    End Function

    Public Function CreateRefreshTokenRequest(ClientId As String, RefreshToken As String) As String
        Return "client_id=" & Uri.EscapeDataString(RequireClientId(ClientId)) &
            "&refresh_token=" & Uri.EscapeDataString(RefreshToken) &
            "&grant_type=refresh_token&scope=" & Uri.EscapeDataString(Scope)
    End Function

End Module
