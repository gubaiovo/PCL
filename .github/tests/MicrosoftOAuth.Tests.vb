Imports System
Imports System.Collections.Generic

Module MicrosoftOAuthTests

    Private Const ClientId As String = "11111111-2222-3333-4444-555555555555"
    Private Const OtherClientId As String = "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"
    Private Checks As Integer

    Sub Main()
        Expect(MicrosoftOAuth.ResolveClientId(ClientId, OtherClientId, "build") = ClientId, "Environment override")
        Expect(MicrosoftOAuth.ResolveClientId("", OtherClientId, ClientId) = OtherClientId, "Runtime setting")
        Expect(MicrosoftOAuth.ResolveClientId("  ", "", ClientId) = ClientId, "Blank environment preserves embedded ID")
        Expect(MicrosoftOAuth.ResolveClientId(Nothing, Nothing, "  " & ClientId & "  ") = ClientId, "Trim embedded ID")
        Expect(MicrosoftOAuth.ResolveClientId(Nothing, " ", Nothing) = "", "Missing configuration")
        Expect(MicrosoftOAuth.RequireClientId("  " & OtherClientId.ToUpperInvariant() & "  ") = OtherClientId, "Normalize GUID")
        Expect(MicrosoftOAuth.RequireClientId("0123456789abcdef") = "0123456789abcdef", "Legacy application ID format")

        ExpectRejected(Sub() MicrosoftOAuth.CreateDeviceCodeRequest(""), "Missing device request ID")
        ExpectRejected(Sub() MicrosoftOAuth.CreateDeviceTokenRequest(" ", "device"), "Missing polling ID")
        ExpectRejected(Sub() MicrosoftOAuth.CreateRefreshTokenRequest(Nothing, "refresh"), "Missing refresh ID")
        ExpectRejected(Sub() MicrosoftOAuth.CreateDeviceCodeRequest("not-an-application-id"), "Invalid ID")

        Dim Initial = FormFields(MicrosoftOAuth.CreateDeviceCodeRequest(ClientId))
        Expect(Initial("client_id") = ClientId, "Device request client ID")
        Expect(Initial("scope") = "XboxLive.signin offline_access", "Device request scope")
        Dim DeviceCode = "device+code&=/?"
        Dim Poll = FormFields(MicrosoftOAuth.CreateDeviceTokenRequest(ClientId, DeviceCode))
        Expect(Poll("client_id") = ClientId, "Polling keeps original client ID")
        Expect(Poll("device_code") = DeviceCode, "Encode device code")
        Expect(Poll("grant_type") = "urn:ietf:params:oauth:grant-type:device_code", "Device grant")
        Dim RefreshToken = "test+refresh&=/?"
        Dim Refresh = FormFields(MicrosoftOAuth.CreateRefreshTokenRequest(ClientId, RefreshToken))
        Expect(Refresh("client_id") = ClientId, "Refresh client ID")
        Expect(Refresh("refresh_token") = RefreshToken, "Encode refresh token")
        Expect(Refresh("grant_type") = "refresh_token", "Refresh grant")
        Expect(Refresh("scope") = Initial("scope"), "Refresh scope")
        Expect(MicrosoftOAuth.TokenEndpoint = "https://login.microsoftonline.com/consumers/oauth2/v2.0/token", "Consistent token authority")
        Console.WriteLine("Microsoft OAuth configuration checks passed: " & Checks)
    End Sub

    Private Sub Expect(Value As Boolean, Label As String)
        If Not Value Then Throw New Exception(Label)
        Checks += 1
    End Sub

    Private Sub ExpectRejected(Action As Action, Label As String)
        Try
            Action()
        Catch ex As InvalidOperationException
            Expect(ex.Message.Contains("Client ID"), Label)
            Return
        End Try
        Throw New Exception(Label)
    End Sub

    Private Function FormFields(Body As String) As Dictionary(Of String, String)
        Dim Result As New Dictionary(Of String, String)
        For Each Entry In Body.Split("&"c)
            Dim Separator = Entry.IndexOf("="c)
            Result.Add(Uri.UnescapeDataString(Entry.Substring(0, Separator)), Uri.UnescapeDataString(Entry.Substring(Separator + 1)))
        Next
        Return Result
    End Function

End Module
