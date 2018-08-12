﻿Imports System

Friend Class FrmAdmin
    Private Sub FrmAdmin_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' set values for admin panel
        cmbSpawnItem.Items.Clear()

        ' Add the names
        For i = 1 To MAX_ITEMS
            cmbSpawnItem.Items.Add(i & ": " & Item(i).Name.Trim)
        Next
    End Sub

#Region "Moderation"
    Private Sub BtnAdminWarpTo_Click(sender As Object, e As EventArgs) Handles btnAdminWarpTo.Click

        If GetPlayerAccess(Myindex) < AdminType.Mapper Then
            AddText("You need to be a high enough staff member to do this!", QColorType.AlertColor)
            Exit Sub
        End If

        ' Check to make sure its a valid map #
        If nudAdminMap.Value > 0 AndAlso nudAdminMap.Value <= MAX_MAPS Then
            Network.WarpTo(nudAdminMap.Value)
        Else
            AddText("Invalid map number.", ColorType.BrightRed)
        End If
    End Sub

    Private Sub BtnAdminBan_Click(sender As Object, e As EventArgs) Handles btnAdminBan.Click
        If GetPlayerAccess(Myindex) < AdminType.Mapper Then
            AddText("You need to be a high enough staff member to do this!", QColorType.AlertColor)
            Exit Sub
        End If

        If txtAdminName.Text.Trim.Length < 1 Then Exit Sub

        Network.SendBan(txtAdminName.Text.Trim)
    End Sub

    Private Sub BtnAdminKick_Click(sender As Object, e As EventArgs) Handles btnAdminKick.Click
        If GetPlayerAccess(Myindex) < AdminType.Mapper Then
            AddText("You need to be a high enough staff member to do this!", QColorType.AlertColor)
            Exit Sub
        End If

        If txtAdminName.Text.Trim.Length < 1 Then Exit Sub

        Network.SendKick(txtAdminName.Text.Trim)
    End Sub

    Private Sub BtnAdminWarp2Me_Click(sender As Object, e As EventArgs) Handles btnAdminWarp2Me.Click
        If GetPlayerAccess(Myindex) < AdminType.Mapper Then
            AddText("You need to be a high enough staff member to do this!", QColorType.AlertColor)
            Exit Sub
        End If

        If txtAdminName.Text.Trim.Length < 1 Then Exit Sub

        
        If Single.TryParse(txtAdminName.Text.Trim, 0) Then Exit Sub

        Network.WarpToMe(txtAdminName.Text.Trim)
    End Sub

    Private Sub BtnAdminWarpMe2_Click(sender As Object, e As EventArgs) Handles btnAdminWarpMe2.Click
        If GetPlayerAccess(Myindex) < AdminType.Mapper Then
            AddText("You need to be a high enough staff member to do this!", QColorType.AlertColor)
            Exit Sub
        End If

        If txtAdminName.Text.Trim.Length < 1 Then
            Exit Sub
        End If

        If Single.TryParse(txtAdminName.Text.Trim, 0) Then
            Exit Sub
        End If

        Network.WarpMeTo(txtAdminName.Text.Trim)
    End Sub

    Private Sub BtnAdminSetAccess_Click(sender As Object, e As EventArgs) Handles btnAdminSetAccess.Click
        If GetPlayerAccess(Myindex) < AdminType.Creator Then
            AddText("You need to be a high enough staff member to do this!", QColorType.AlertColor)
            Exit Sub
        End If

        If txtAdminName.Text.Trim.Length < 2 Then
            Exit Sub
        End If

        If Single.TryParse(txtAdminName.Text.Trim, 0) OrElse cmbAccess.SelectedIndex < 0 Then
            Exit Sub
        End If

        Network.SendSetAccess(txtAdminName.Text.Trim, cmbAccess.SelectedIndex)
    End Sub

    Private Sub BtnAdminSetSprite_Click(sender As Object, e As EventArgs) Handles btnAdminSetSprite.Click
        If GetPlayerAccess(Myindex) < AdminType.Mapper Then
            AddText("You need to be a high enough staff member to do this!", QColorType.AlertColor)
            Exit Sub
        End If

        If nudAdminSprite.Value < 1 Then Exit Sub

        Network.SendSetSprite(nudAdminSprite.Value)
    End Sub
#End Region

#Region "Editors"
    Private Sub BtnMapEditor_Click(sender As Object, e As EventArgs) Handles btnMapEditor.Click
        If GetPlayerAccess(Myindex) < AdminType.Mapper Then
            AddText("You need to be a high enough staff member to do this!", QColorType.AlertColor)
            Exit Sub
        End If

        SendRequestEditMap()
    End Sub
#End Region

#Region "Map Report"
    Private Sub BtnMapReport_Click(sender As Object, e As EventArgs) Handles btnMapReport.Click
        If GetPlayerAccess(Myindex) < AdminType.Mapper Then
            AddText("You need to be a high enough staff member to do this!", QColorType.AlertColor)
            Exit Sub
        End If
        Network.SendRequestMapreport()
    End Sub

    Private Sub LstMaps_DoubleClick(sender As Object, e As EventArgs) Handles lstMaps.DoubleClick
        If GetPlayerAccess(Myindex) < AdminType.Mapper Then
            AddText("You need to be a high enough staff member to do this!", QColorType.AlertColor)
            Exit Sub
        End If

        ' Check to make sure its a valid map #
        If lstMaps.FocusedItem.Index + 1 > 0 AndAlso lstMaps.FocusedItem.Index + 1 <= MAX_MAPS Then
            Network.WarpTo(lstMaps.FocusedItem.Index + 1)
        Else
            AddText("Invalid map number: " & lstMaps.FocusedItem.Index + 1, QColorType.AlertColor)
        End If
    End Sub
#End Region

#Region "Misc"
    Private Sub CmbSpawnItem_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbSpawnItem.SelectedIndexChanged
        If Item(cmbSpawnItem.SelectedIndex + 1).Type = ItemType.Currency OrElse Item(cmbSpawnItem.SelectedIndex + 1).Stackable = 1 Then
            nudSpawnItemAmount.Enabled = True
            Exit Sub
        End If
        nudSpawnItemAmount.Enabled = False
    End Sub

    Private Sub BtnSpawnItem_Click(sender As Object, e As EventArgs) Handles btnSpawnItem.Click
        If GetPlayerAccess(Myindex) < AdminType.Creator Then
            AddText("You need to be a high enough staff member to do this!", QColorType.AlertColor)
            Exit Sub
        End If

        Network.SendSpawnItem(cmbSpawnItem.SelectedIndex + 1, nudSpawnItemAmount.Value)
    End Sub

    Private Sub BtnLevelUp_Click(sender As Object, e As EventArgs) Handles btnLevelUp.Click
        If GetPlayerAccess(Myindex) < AdminType.Developer Then
            AddText("You need to be a high enough staff member to do this!", QColorType.AlertColor)
            Exit Sub
        End If

        Network.SendRequestLevelUp()

    End Sub

    Private Sub BtnALoc_Click(sender As Object, e As EventArgs) Handles btnALoc.Click
        If GetPlayerAccess(Myindex) < AdminType.Mapper Then
            AddText("You need to be a high enough staff member to do this!", QColorType.AlertColor)
            Exit Sub
        End If

        BLoc = Not BLoc
    End Sub

    Private Sub BtnRespawn_Click(sender As Object, e As EventArgs) Handles btnRespawn.Click
        If GetPlayerAccess(Myindex) < AdminType.Mapper Then
            AddText("You need to be a high enough staff member to do this!", QColorType.AlertColor)
            Exit Sub
        End If

        SendMapRespawn()
    End Sub

#End Region

End Class