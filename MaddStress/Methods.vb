Imports System.Net.Sockets
Imports System.Net


Module Methods


    Public Sub updateFavorites()

        Dim i As Integer
        MainHome.MainListFAVORITE.Items.Clear()
            For i = 0 To My.Settings.FavIP.Count - 1
                MainHome.MainListFAVORITE.Items.Add(My.Settings.FavIP.Item(i).ToString & " -- " & My.Settings.FavName.Item(i).ToString & " -- " & My.Settings.FavNotes.Item(i).ToString)
        Next
        My.Settings.Save()
        updateFavCB()

    End Sub


    Public Sub updateHistory()

        MainHome.MainListHistory.Items.Clear()
        Dim i As Integer
        For i = 0 To My.Settings.History.Count - 1
            MainHome.MainListHistory.Items.Add(My.Settings.History.Item(i))
        Next
        My.Settings.Save()

    End Sub


    Public Sub addHistoryItem(ByVal StringToAdd As String)

        My.Settings.History.Add(StringToAdd)
        updateHistory()

    End Sub


    Public Sub addFavoriteItem(ByVal IP As String, ByVal Name As String, ByVal notes As String)

        My.Settings.FavIP.Add(IP)
        My.Settings.FavName.Add(Name)
        My.Settings.FavNotes.Add(notes)
        updateFavorites()

    End Sub


    Public Sub updateTF()

        MainHome.MainLabelTOTALFLOOD.Text = "Total Floods: " & My.Settings.TotalFloods

    End Sub


    Public Sub updateFavCB()

        Dim i As Integer
        MainHome.MainComboFAVORITES.Items.Clear()
        For i = 0 To My.Settings.FavIP.Count - 1
            MainHome.MainComboFAVORITES.Items.Add(My.Settings.FavName.Item(i).ToString)
        Next
        My.Settings.Save()

    End Sub


    Public Sub DefaultSetUp()

        If My.Settings.DBytes > 0 Then
            MainHome.MainTextSYNBYTES.Text = My.Settings.DBytes
            MainHome.MainTextSETBYTES.Text = My.Settings.DBytes
        End If
        If My.Settings.DPackets > 0 Then
            MainHome.MainTextUDPPACKETS.Text = My.Settings.DPackets
            MainHome.MainTextTCPPACKETS.Text = My.Settings.DPackets
            MainHome.MainTextSETPACKETS.Text = My.Settings.DPackets
        End If
        If My.Settings.DPort > 0 Then
            MainHome.MainTextSETPORT.Text = My.Settings.DPort
            MainHome.MainTextUDPPORT.Text = My.Settings.DPort
            MainHome.MainTextTCPPORT.Text = My.Settings.DPort
        End If
        If My.Settings.DSockets > 0 Then
            MainHome.MainTextUDPSOCKETS.Text = My.Settings.DSockets
            MainHome.MainTextTCPSOCKETS.Text = My.Settings.DSockets
            MainHome.MainTextSETSOCKETS.Text = My.Settings.DSockets
        End If

    End Sub


    Public Sub ScanPort(ByVal PORT As Integer)

        Try
            Dim tmpClient As New TcpClient()
            Dim tmpEndPoint As New IPEndPoint(IPAddress.Parse(MainHome.MainTextIPPORTSCANNER.Text), PORT)
            tmpClient.Connect(tmpEndPoint)
            Threading.Thread.Sleep(50)
            If tmpClient.Connected = True Then
                MainHome.MainListPORTSCANNER.Items.Add("port " & PORT & " is open at, " & MainHome.MainTextIPPORTSCANNER.Text)
                My.Settings.History.Add("port " & PORT & " is open at, " & MainHome.MainTextIPPORTSCANNER.Text)
                addHistoryItem("port " & PORT & " is open at, " & MainHome.MainTextIPPORTSCANNER.Text)
            End If
        Catch ex As Exception
        End Try
    End Sub

End Module