﻿Imports System.ComponentModel
Imports SFML.Graphics
Imports SFML.Window

Friend Class frmMapEditor
#Region "Form Code"

    Private Sub FrmEditor_Map_Load(sender As Object, e As EventArgs) Handles MyBase.Shown
        cmbTileSets.SelectedIndex = 0
        pnlAttributes.BringToFront()
        pnlAttributes.Visible = False
        pnlTiles.BringToFront()
        pnlTiles.Visible = True
        optBlocked.Checked = True
        SelectedTab = 1

        nudFog.Maximum = NumFogs

        rsMap.Width = (Map.MaxX + 1) * PIC_X
        rsMap.Height = (Map.MaxY + 1) * PIC_Y

    End Sub

    Private Sub FrmEditor_MapEditor_Closing(sender As Object, e As CancelEventArgs) Handles MyBase.Closing
        MapEditorCancel()
    End Sub

    Private Sub FrmEditor_MapEditor_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        MapEditorCancel()
    End Sub

    Private Sub CmbTileSets_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbTileSets.SelectedIndexChanged
        If cmbTileSets.SelectedIndex + 1 > NumTileSets Then
            cmbTileSets.SelectedIndex = 0
        End If

        Map.tileset = cmbTileSets.SelectedIndex + 1

        EditorTileSelStart = New Point(0, 0)
        EditorTileSelEnd = New Point(1, 1)
    End Sub

    Private Sub CmbAutoTile_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbAutoTile.SelectedIndexChanged
        If cmbAutoTile.SelectedIndex = 0 Then
            EditorTileWidth = 1
            EditorTileHeight = 1
        End If
    End Sub

    Private Sub BtnTiles_Click(sender As Object, e As EventArgs) Handles btnTiles.Click
        SelectedTab = 1

        pnlTiles.Visible = True

        pnlAttribute.Visible = False
        pnlNpc.Visible = False
        pnlDirBlock.Visible = False
        pnlEvents.Visible = False
    End Sub

    Private Sub BtnAttributes_Click(sender As Object, e As EventArgs) Handles btnAttributes.Click
        SelectedTab = 2

        pnlAttribute.Visible = True

        pnlTiles.Visible = False
        pnlNpc.Visible = False
        pnlDirBlock.Visible = False
        pnlEvents.Visible = False
    End Sub

    Private Sub BtnNpc_Click(sender As Object, e As EventArgs) Handles btnNpc.Click
        SelectedTab = 3

        pnlNpc.Visible = True

        pnlTiles.Visible = False
        pnlAttribute.Visible = False
        pnlDirBlock.Visible = False
        pnlEvents.Visible = False
    End Sub

    Private Sub BtnDirBlock_Click(sender As Object, e As EventArgs) Handles btnDirBlock.Click
        SelectedTab = 4

        pnlDirBlock.Visible = True

        pnlTiles.Visible = False
        pnlNpc.Visible = False
        pnlAttribute.Visible = False
        pnlEvents.Visible = False
    End Sub

    Private Sub BtnEvents_Click(sender As Object, e As EventArgs) Handles btnEvents.Click
        SelectedTab = 5

        pnlEvents.Visible = True

        pnlTiles.Visible = False
        pnlAttribute.Visible = False
        pnlNpc.Visible = False
        pnlDirBlock.Visible = False

    End Sub
#End Region

#Region "Toolbar"
    Private Sub TsbSave_Click(sender As Object, e As EventArgs) Handles tsbSave.Click
        HideCursor = True
        ScreenShotTimer = GetTickCount() + 500
        MakeCache = True
    End Sub

    Private Sub TsbDiscard_Click(sender As Object, e As EventArgs) Handles tsbDiscard.Click
        MapEditorCancel()
    End Sub

    Private Sub TsbMapGrid_Click(sender As Object, e As EventArgs) Handles tsbMapGrid.Click
        MapGrid = Not MapGrid
    End Sub

    Private Sub TsbFill_Click(sender As Object, e As EventArgs) Handles tsbFill.Click
        MapEditorFillLayer(cmbAutoTile.SelectedIndex)
    End Sub

    Private Sub TsbClear_Click(sender As Object, e As EventArgs) Handles tsbClear.Click
        MapEditorClearLayer()
    End Sub

    Private Sub CmbMapList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbMapList.SelectedIndexChanged
        SendEditorRequestMap(cmbMapList.SelectedIndex + 1)
    End Sub

    Private Sub TsbScreenShot_Click(sender As Object, e As EventArgs) Handles tsbScreenShot.Click
        HideCursor = True
        ScreenShotTimer = GetTickCount() + 1000
        TakeScreenShot = True
    End Sub
#End Region

#Region "RsMap"

    Private Sub rsMap_Render(sender As Object, e As EventArgs) Handles rsMap.Render
        Dim X As Integer, Y As Integer, I As Integer

        'Don't Render IF
        If GettingMap Then Exit Sub
        'lets get going

        'update view around player
        UpdateCamera()
        
        rsMap.Width = (Map.MaxX + 1) * PIC_X
        rsMap.Height = (Map.MaxY + 1) * PIC_Y
<<<<<<< HEAD
        rsMap.RenderWindow.DispatchEvents()
        rsMap.View = New View(New FloatRect(0, 0,
            rsMap.Width, rsMap.Height))

=======
        
>>>>>>> parent of 9300314... just some renamin shit.
        'clear any unused gfx
        ClearGFX()

        ' update animation editor
        'If Editor = EDITOR_ANIMATION Then
        '    EditorAnim_DrawAnim()
        'End If

        If InMapEditor AndAlso MapData = True Then
            ' blit lower tiles
            If NumTileSets > 0 Then
                For X = 0 To Map.MaxX + 1
                    For Y = 0 To Map.MaxY + 1
                        If IsValidMapPoint(X, Y) Then
                            DrawMapTile(X, Y)
                        End If
                    Next
                Next
            End If

            ' events
            If Map.CurrentEvents > 0 AndAlso Map.CurrentEvents <= Map.EventCount Then

                For I = 1 To Map.CurrentEvents
                    If Map.MapEvents(I).Position = 0 Then
                        DrawEvent(I)
                    End If
                Next
            End If

            ' Draw out the items
            If NumItems > 0 Then
                For I = 1 To MAX_MAP_ITEMS

                    If MapItem(I).Num > 0 Then
                        DrawItem(I)
                    End If

                Next
            End If

            'Draw sum d00rs.
            For X = 0 To Map.MaxX
                For Y = 0 To Map.MaxY

                    If IsValidMapPoint(X, Y) Then
                        If Map.Tile(X, Y).Type = TileType.Door Then
                            DrawDoor(X, Y)
                        End If
                    End If

                Next
            Next

            ' Y-based render. Renders Players, Npcs and Resources based on Y-axis.
            For Y = 0 To Map.MaxY

                If NumCharacters > 0 Then

                    ' Npcs
                    For I = 1 To MAX_MAP_NPCS
                        If MapNpc(I).Y = Y Then
                            DrawNpc(I)
                        End If
                    Next

                    ' events
                    If Map.CurrentEvents > 0 AndAlso Map.CurrentEvents <= Map.EventCount Then

                        For I = 1 To Map.CurrentEvents
                            If Map.MapEvents(I).Position = 1 Then
                                If Y = Map.MapEvents(I).Y Then
                                    DrawEvent(I)
                                End If
                            End If
                        Next
                    End If

                End If

                ' Resources
                If NumResources > 0 Then
                    If Resources_Init Then
                        If Resource_Index > 0 Then
                            For I = 1 To Resource_Index
                                If MapResource(I).Y = Y Then
                                    DrawMapResource(I)
                                End If
                            Next
                        End If
                    End If
                End If
            Next

            'events
            If Map.CurrentEvents > 0 AndAlso Map.CurrentEvents <= Map.EventCount Then

                For I = 1 To Map.CurrentEvents
                    If Map.MapEvents(I).Position = 2 Then
                        DrawEvent(I)
                    End If
                Next
            End If

            ' blit out upper tiles
            If NumTileSets > 0 Then
                For X = 0 To Map.MaxX + 1
                    For Y = 0 To Map.MaxY + 1
                        If IsValidMapPoint(X, Y) Then
                            DrawMapFringeTile(X, Y)
                        End If
                    Next
                Next
            End If

            DrawWeather()
            DrawThunderEffect()
            DrawMapTint()

            ' Draw out a square at mouse cursor
            If MapGrid = True Then
                DrawGrid()
            End If

            If SelectedTab = 4 Then
                For X = 0 To Map.MaxX
                    For Y = 0 To Map.MaxY
                        If IsValidMapPoint(X, Y) Then
                            DrawDirections(X, Y)
                        End If
                    Next
                Next
            End If

            'draw event names
            For I = 0 To Map.CurrentEvents
                If Map.MapEvents(I).Visible = 1 Then
                    If Map.MapEvents(I).ShowName = 1 Then
                        DrawEventName(I)
                    End If
                End If
            Next

            ' draw npc names
            For I = 1 To MAX_MAP_NPCS
                If MapNpc(I).Num > 0 Then
                    DrawNPCName(I)
                End If
            Next

            If CurrentFog > 0 Then
                DrawFog()
            End If

            ' Blit out map attributes
            If InMapEditor Then
                DrawMapAttributes()
                DrawTileOutline()
            End If

            If InMapEditor AndAlso SelectedTab = 5 Then
                DrawEvents()
                EditorEvent_DrawGraphic()
            End If

            ' Draw map name
            DrawMapName()
        End If
    End Sub

<<<<<<< HEAD
    Private Sub RsMap_MouseDown(sender As Object, e As MouseEventArgs) Handles rsMap.MouseDown
        If e.X > rsMap.Width - 32 OrElse e.Y > rsMap.Height - 32 Then Exit Sub
=======
    Private Sub rsMap_MouseDown(sender As Object, e As MouseEventArgs) Handles rsMap.MouseDown
        If e.X > pnlBack2.Width - 32 OrElse e.Y > pnlBack2.Height - 32 Then Exit Sub
>>>>>>> parent of 9300314... just some renamin shit.
        MapEditorMouseDown(e.Button, e.X, e.Y, False)

    End Sub

<<<<<<< HEAD
    Private Sub RsMap_MouseMove(sender As Object, e As MouseEventArgs) Handles rsMap.MouseMove
=======
    Private Overloads Sub rsMap_Paint(sender As Object, e As PaintEventArgs) Handles rsMap.Paint
        'This is here to make sure that the box dosen't try to re-paint itself... saves time and w/e else
        Exit Sub
    End Sub

    Private Sub rsMap_MouseMove(sender As Object, e As MouseEventArgs) Handles rsMap.MouseMove
>>>>>>> parent of 9300314... just some renamin shit.

        CurX = e.Location.X \ PIC_X
        CurY = e.Location.Y \ PIC_Y

        CurMouseX = e.Location.X
        CurMouseY = e.Location.Y

        If e.Button = MouseButtons.Left OrElse e.Button = MouseButtons.Right Then
            MapEditorMouseDown(e.Button, e.X, e.Y)
        End If

        tslCurXY.Text = "X: " & CurX & " - " & " Y: " & CurY
    End Sub

    Private Sub rsMap_MouseUp(sender As Object, e As MouseEventArgs) Handles rsMap.MouseUp

        CurX = e.Location.X \ PIC_X
        CurY = e.Location.Y \ PIC_Y

    End Sub

#End Region

#Region "RsTileset"
    Private Sub RsTileset_Render(sender As Object, e As EventArgs) Handles rsTileset.Render
        Dim height As Integer
        Dim width As Integer
        Dim tileset As Byte

        ' find tileset number
        tileset = cmbTileSets.SelectedIndex + 1

        ' exit out if doesn't exist
        If tileset <= 0 OrElse tileset > NumTileSets Then Exit Sub

        If TileSetTextureInfo(tileset).IsLoaded = False Then
            LoadTexture(tileset, 1)
        End If
        ' we use it, lets update timer
        With TileSetTextureInfo(tileset)
            .TextureTimer = GetTickCount() + 100000
        End With

        rsTileset.Width = TileSetSprite(tileset).Texture.Size.X
        rsTileset.Height = TileSetSprite(tileset).Texture.Size.Y
        rsTileset.RenderWindow.DispatchEvents()
        rsTileset.View = New View(New FloatRect(0, 0,
            rsTileset.RenderWindow.Size.X, rsTileset.RenderWindow.Size.Y))

        Dim rec2 As New RectangleShape With {
            .OutlineColor = New Color(Color.Red),
            .OutlineThickness = 0.6,
            .FillColor = New Color(Color.Transparent)
        }



        height = TileSetTextureInfo(tileset).height
        width = TileSetTextureInfo(tileset).width

        ' change selected shape for autotiles
        If cmbAutoTile.SelectedIndex > 0 Then
            Select Case cmbAutoTile.SelectedIndex
                Case 1 ' autotile
                    EditorTileWidth = 2
                    EditorTileHeight = 3
                Case 2 ' fake autotile
                    EditorTileWidth = 1
                    EditorTileHeight = 1
                Case 3 ' animated
                    EditorTileWidth = 6
                    EditorTileHeight = 3
                Case 4 ' cliff
                    EditorTileWidth = 2
                    EditorTileHeight = 2
                Case 5 ' waterfall
                    EditorTileWidth = 2
                    EditorTileHeight = 3
            End Select
        End If

        RenderSprite(TileSetSprite(tileset), rsTileset, 0, 0, 0, 0, width, height)

        rec2.Size = New Vector2f(EditorTileWidth * PIC_X, EditorTileHeight * PIC_Y)

        rec2.Position = New Vector2f(EditorTileSelStart.X * PIC_X, EditorTileSelStart.Y * PIC_Y)
        rsTileset.Draw(rec2)

        LastTileset = tileset
    End Sub

    Private Sub RsTileset_MouseDown(sender As Object, e As MouseEventArgs) Handles rsTileset.MouseDown
        MapEditorChooseTile(e.Button, e.X, e.Y)
    End Sub

    Private Sub RsTileset_MouseMove(sender As Object, e As MouseEventArgs) Handles rsTileset.MouseMove
        MapEditorDrag(e.Button, e.X, e.Y)
    End Sub
#End Region

#Region "Attributes"
    Private Sub LblVisualWarp_Click(sender As Object, e As EventArgs) Handles lblVisualWarp.Click
        fraMapWarp.Visible = False
        FrmVisualWarp.Visible = True
    End Sub

    Private Sub ScrlMapWarpMap_Scroll(sender As Object, e As ScrollEventArgs) Handles scrlMapWarpMap.Scroll
        lblMapWarpMap.Text = "Map: " & scrlMapWarpMap.Value
    End Sub

    Private Sub ScrlMapWarpX_Scroll(sender As Object, e As ScrollEventArgs) Handles scrlMapWarpX.Scroll
        lblMapWarpX.Text = "X: " & scrlMapWarpX.Value
    End Sub

    Private Sub ScrlMapWarpY_Scroll(sender As Object, e As ScrollEventArgs) Handles scrlMapWarpY.Scroll
        lblMapWarpY.Text = "Y: " & scrlMapWarpY.Value
    End Sub

    Private Sub BtnMapWarp_Click(sender As Object, e As EventArgs) Handles btnMapWarp.Click
        EditorWarpMap = scrlMapWarpMap.Value

        EditorWarpX = scrlMapWarpX.Value
        EditorWarpY = scrlMapWarpY.Value
        pnlAttributes.Visible = False
        fraMapWarp.Visible = False
    End Sub

    Private Sub OptWarp_CheckedChanged(sender As Object, e As EventArgs) Handles optWarp.CheckedChanged

        If optWarp.Checked = True Then
            ClearAttributeDialogue()
            fraMapWarp.Visible = True
            'FrmVisualWarp.Visible = True
        End If

    End Sub

    Private Sub ScrlMapItem_Scroll(sender As Object, e As ScrollEventArgs) Handles scrlMapItem.Scroll
        If Item(scrlMapItem.Value).Type = ItemType.Currency OrElse Item(scrlMapItem.Value).Stackable = 1 Then
            scrlMapItemValue.Enabled = True
        Else
            scrlMapItemValue.Value = 1
            scrlMapItemValue.Enabled = False
        End If

        EditorMap_DrawMapItem()
        lblMapItem.Text = "Item: " & scrlMapItem.Value & ". " & Trim$(Item(scrlMapItem.Value).Name) & " x" & scrlMapItemValue.Value
    End Sub

    Private Sub ScrlMapItemValue_Scroll(sender As Object, e As ScrollEventArgs) Handles scrlMapItemValue.Scroll
        lblMapItem.Text = Trim$(Item(scrlMapItem.Value).Name) & " x" & scrlMapItemValue.Value
    End Sub

    Private Sub BtnMapItem_Click(sender As Object, e As EventArgs) Handles btnMapItem.Click
        ItemEditorNum = scrlMapItem.Value
        ItemEditorValue = scrlMapItemValue.Value
        pnlAttributes.Visible = False
        fraMapItem.Visible = False
    End Sub

    Private Sub OptItem_CheckedChanged(sender As Object, e As EventArgs) Handles optItem.CheckedChanged
        ClearAttributeDialogue()
        pnlAttributes.Visible = True
        fraMapItem.Visible = True

        scrlMapItem.Maximum = MAX_ITEMS
        scrlMapItem.Value = 1
        lblMapItem.Text = Trim$(Item(scrlMapItem.Value).Name) & " x" & scrlMapItemValue.Value
        EditorMap_DrawMapItem()
    End Sub

    Private Sub ScrlMapKey_Scroll(sender As Object, e As ScrollEventArgs) Handles scrlMapKey.Scroll
        lblMapKey.Text = "Item: " & Trim$(Item(scrlMapKey.Value).Name)
        EditorMap_DrawKey()
    End Sub

    Private Sub BtnMapKey_Click(sender As Object, e As EventArgs) Handles btnMapKey.Click
        KeyEditorNum = scrlMapKey.Value
        KeyEditorTake = chkMapKey.Checked
        pnlAttributes.Visible = False
        fraMapKey.Visible = False
    End Sub

    Private Sub OptKey_CheckedChanged(sender As Object, e As EventArgs) Handles optKey.CheckedChanged
        ClearAttributeDialogue()
        pnlAttributes.Visible = True
        fraMapKey.Visible = True

        scrlMapKey.Maximum = MAX_ITEMS
        scrlMapKey.Value = 1
        chkMapKey.Checked = True
        EditorMap_DrawKey()
        lblMapKey.Text = "Item: " & Trim$(Item(scrlMapKey.Value).Name)
    End Sub

    Private Sub ScrlKeyX_Scroll(sender As Object, e As ScrollEventArgs) Handles scrlKeyX.Scroll
        lblKeyX.Text = "X: " & scrlKeyX.Value
    End Sub

    Private Sub ScrlKeyY_Scroll(sender As Object, e As ScrollEventArgs) Handles scrlKeyY.Scroll
        lblKeyY.Text = "X: " & scrlKeyY.Value
    End Sub

    Private Sub BtnMapKeyOpen_Click(sender As Object, e As EventArgs) Handles btnMapKeyOpen.Click
        KeyOpenEditorX = scrlKeyX.Value
        KeyOpenEditorY = scrlKeyY.Value
        pnlAttributes.Visible = False
        fraKeyOpen.Visible = False
    End Sub

    Private Sub OptKeyOpen_CheckedChanged(sender As Object, e As EventArgs) Handles optKeyOpen.CheckedChanged
        ClearAttributeDialogue()
        fraKeyOpen.Visible = True
        pnlAttributes.Visible = True

        scrlKeyX.Maximum = Map.MaxX
        scrlKeyY.Maximum = Map.MaxY
        scrlKeyX.Value = 0
        scrlKeyY.Value = 0
    End Sub

    Private Sub BtnResourceOk_Click(sender As Object, e As EventArgs) Handles btnResourceOk.Click
        ResourceEditorNum = scrlResource.Value
        pnlAttributes.Visible = False
        fraResource.Visible = False
    End Sub

    Private Sub ScrlResource_Scroll(sender As Object, e As ScrollEventArgs) Handles scrlResource.Scroll
        lblResource.Text = "Resource: " & Resource(scrlResource.Value).Name
    End Sub

    Private Sub OptResource_CheckedChanged(sender As Object, e As EventArgs) Handles optResource.CheckedChanged
        ClearAttributeDialogue()
        pnlAttributes.Visible = True
        fraResource.Visible = True
    End Sub

    Private Sub BtnNpcSpawn_Click(sender As Object, e As EventArgs) Handles btnNpcSpawn.Click
        SpawnNpcNum = lstNpc.SelectedIndex + 1
        SpawnNpcDir = scrlNpcDir.Value
        pnlAttributes.Visible = False
        fraNpcSpawn.Visible = False
    End Sub

    Private Sub OptNPCSpawn_CheckedChanged(sender As Object, e As EventArgs) Handles optNpcSpawn.CheckedChanged
        Dim n As Integer

        lstNpc.Items.Clear()

        For n = 1 To MAX_MAP_NPCS
            If Map.Npc(n) > 0 Then
                lstNpc.Items.Add(n & ": " & Npc(Map.Npc(n)).Name)
            Else
                lstNpc.Items.Add(n & ": No Npc")
            End If
        Next n

        scrlNpcDir.Value = 0
        lstNpc.SelectedIndex = 0

        ClearAttributeDialogue()
        pnlAttributes.Visible = True
        fraNpcSpawn.Visible = True
    End Sub

    Private Sub BtnShop_Click(sender As Object, e As EventArgs) Handles btnShop.Click
        EditorShop = cmbShop.SelectedIndex
        pnlAttributes.Visible = False
        fraShop.Visible = False
    End Sub

    Private Sub OptShop_CheckedChanged(sender As Object, e As EventArgs) Handles optShop.CheckedChanged
        ClearAttributeDialogue()
        pnlAttributes.Visible = True
        fraShop.Visible = True
    End Sub

    Private Sub BtnHeal_Click(sender As Object, e As EventArgs) Handles btnHeal.Click
        MapEditorHealType = cmbHeal.SelectedIndex + 1
        MapEditorHealAmount = scrlHeal.Value
        pnlAttributes.Visible = False
        fraHeal.Visible = False
    End Sub

    Private Sub ScrlHeal_Scroll(sender As Object, e As ScrollEventArgs) Handles scrlHeal.Scroll
        lblHeal.Text = "Amount: " & scrlHeal.Value
    End Sub

    Private Sub OptHeal_CheckedChanged(sender As Object, e As EventArgs) Handles optHeal.CheckedChanged
        ClearAttributeDialogue()
        pnlAttributes.Visible = True
        fraHeal.Visible = True
    End Sub

    Private Sub ScrlTrap_Scroll(sender As Object, e As ScrollEventArgs) Handles scrlTrap.Scroll
        lblTrap.Text = "Amount: " & scrlTrap.Value
    End Sub

    Private Sub BtnTrap_Click(sender As Object, e As EventArgs) Handles btnTrap.Click
        MapEditorHealAmount = scrlTrap.Value
        pnlAttributes.Visible = False
        fraTrap.Visible = False
    End Sub

    Private Sub OptTrap_CheckedChanged(sender As Object, e As EventArgs) Handles optTrap.CheckedChanged
        ClearAttributeDialogue()
        pnlAttributes.Visible = True
        fraTrap.Visible = True
    End Sub

    Private Sub BtnClearAttribute_Click(sender As Object, e As EventArgs) Handles btnClearAttribute.Click
        MapEditorClearAttribs()
    End Sub

    Private Sub ScrlNpcDir_Scroll(sender As Object, e As ScrollEventArgs) Handles scrlNpcDir.Scroll
        Select Case scrlNpcDir.Value
            Case 0
                lblNpcDir.Text = "Direction: Up"
            Case 1
                lblNpcDir.Text = "Direction: Down"
            Case 2
                lblNpcDir.Text = "Direction: Left"
            Case 3
                lblNpcDir.Text = "Direction: Right"
        End Select
    End Sub

    Private Sub OptBlocked_CheckedChanged(sender As Object, e As EventArgs) Handles optBlocked.CheckedChanged
        If optBlocked.Checked Then pnlAttributes.Visible = False
    End Sub

    Private Sub OptHouse_CheckedChanged(sender As Object, e As EventArgs) Handles optHouse.CheckedChanged
        ClearAttributeDialogue()
        pnlAttributes.Visible = True
        fraBuyHouse.Visible = True
        scrlBuyHouse.Maximum = MAX_HOUSES
        scrlBuyHouse.Value = 1
    End Sub

    Private Sub ScrlBuyHouse_Scroll(sender As Object, e As ScrollEventArgs) Handles scrlBuyHouse.Scroll
        lblHouseName.Text = scrlBuyHouse.Value & ". " & HouseConfig(scrlBuyHouse.Value).ConfigName
    End Sub

    Private Sub BtnHouseTileOk_Click(sender As Object, e As EventArgs) Handles btnHouseTileOk.Click
        HouseTileindex = scrlBuyHouse.Value
        pnlAttributes.Visible = False
        fraBuyHouse.Visible = False
    End Sub

#End Region

#Region "Map Settings"
    Private Sub TxtName_TextChanged(sender As Object, e As EventArgs) Handles txtName.TextChanged
        Map.Name = Trim$(txtName.Text)
    End Sub

    Private Sub CmbMoral_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbMoral.SelectedIndexChanged
        Map.Moral = cmbMoral.SelectedIndex
    End Sub

    Private Sub NudLeft_ValueChanged(sender As Object, e As EventArgs) Handles nudLeft.ValueChanged
        Map.Left = nudLeft.Value
    End Sub

    Private Sub NudRight_ValueChanged(sender As Object, e As EventArgs) Handles nudRight.ValueChanged
        Map.Right = nudRight.Value
    End Sub

    Private Sub NudUp_ValueChanged(sender As Object, e As EventArgs) Handles nudUp.ValueChanged
        Map.Up = nudUp.Value
    End Sub

    Private Sub NudDown_ValueChanged(sender As Object, e As EventArgs) Handles nudDown.ValueChanged
        Map.Down = nudDown.Value
    End Sub

    Private Sub TxtBootMap_TextChanged(sender As Object, e As EventArgs) Handles nudSpawnMap.ValueChanged
        Map.BootMap = nudSpawnMap.Value
    End Sub

    Private Sub TxtBootX_TextChanged(sender As Object, e As EventArgs) Handles nudSpawnX.ValueChanged
        Map.BootX = nudSpawnX.Value
    End Sub

    Private Sub TxtBootY_TextChanged(sender As Object, e As EventArgs) Handles nudSpawnY.ValueChanged
        Map.BootY = nudSpawnY.Value
    End Sub

    Private Sub BtnPreview_Click(sender As Object, e As EventArgs) Handles btnPreview.Click
        If PreviewPlayer Is Nothing Then
            If lstMusic.SelectedIndex >= 0 Then
                StopMusic()
                PlayPreview(lstMusic.Items(lstMusic.SelectedIndex).ToString)
            End If
        Else
            StopPreview()
            PlayMusic(Map.Music)
        End If
    End Sub

    Private Sub CmbWeather_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbWeather.SelectedIndexChanged
        Map.WeatherType = cmbWeather.SelectedIndex
        CurrentWeather = cmbWeather.SelectedIndex
    End Sub

    Private Sub ScrlFog_Scroll(sender As Object, e As EventArgs) Handles nudFog.ValueChanged
        Map.Fogindex = nudFog.Value
        CurrentFog = nudFog.Value
    End Sub

    Private Sub ScrlIntensity_Scroll(sender As Object, e As EventArgs) Handles nudIntensity.ValueChanged
        Map.WeatherIntensity = nudIntensity.Value
        CurrentWeatherIntensity = nudIntensity.Value
    End Sub

    Private Sub ScrlFogSpeed_Scroll(sender As Object, e As EventArgs) Handles nudFogSpeed.ValueChanged
        Map.FogSpeed = nudFogSpeed.Value
        CurrentFogSpeed = nudFogSpeed.Value
    End Sub

    Private Sub ScrlFogAlpha_Scroll(sender As Object, e As EventArgs) Handles nudFogAlpha.ValueChanged
        Map.FogAlpha = nudFogAlpha.Value
        CurrentFogOpacity = nudFogAlpha.Value
    End Sub

    Private Sub ChkUseTint_CheckedChanged(sender As Object, e As EventArgs) Handles chkUseTint.CheckedChanged
        If chkUseTint.Checked = True Then
            Map.HasMapTint = 1
        Else
            Map.HasMapTint = 0
        End If
    End Sub

    Private Sub ScrlMapRed_Scroll(sender As Object, e As EventArgs) Handles nudMapRed.ValueChanged
        Map.MapTintR = nudMapRed.Value
        CurrentTintR = nudMapRed.Value
    End Sub

    Private Sub ScrlMapGreen_Scroll(sender As Object, e As EventArgs) Handles nudMapGreen.ValueChanged
        Map.MapTintG = nudMapGreen.Value
        CurrentTintG = nudMapGreen.Value
    End Sub

    Private Sub ScrlMapBlue_Scroll(sender As Object, e As EventArgs) Handles nudMapBlue.ValueChanged
        Map.MapTintB = nudMapBlue.Value
        CurrentTintB = nudMapBlue.Value
    End Sub

    Private Sub ScrlMapAlpha_Scroll(sender As Object, e As EventArgs) Handles nudMapAlpha.ValueChanged
        Map.MapTintA = nudMapAlpha.Value
        CurrentTintA = nudMapAlpha.Value
    End Sub

    Private Sub BtnSetSize_Click(sender As Object, e As EventArgs) Handles btnSetSize.Click
        Dim X As Integer, x2 As Integer, i As Integer
        Dim Y As Integer, y2 As Integer
        Dim tempArr(,) As TileRec

        If nudMaxX.Value < SCREEN_MAPX Then nudMaxX.Value = SCREEN_MAPX
        If nudMaxY.Value < SCREEN_MAPY Then nudMaxY.Value = SCREEN_MAPY

        GettingMap = True
        With Map

            ' set the data before changing it
            ReDim tempArr(.MaxX, .MaxY)
            For X = 0 To .MaxX
                For Y = 0 To .MaxY
                    ReDim tempArr(X, Y).Layer(LayerType.Count - 1)

                    tempArr(X, Y).Data1 = .Tile(X, Y).Data1
                    tempArr(X, Y).Data2 = .Tile(X, Y).Data2
                    tempArr(X, Y).Data3 = .Tile(X, Y).Data3
                    tempArr(X, Y).DirBlock = .Tile(X, Y).DirBlock
                    tempArr(X, Y).Type = .Tile(X, Y).Type

                    For i = 1 To LayerType.Count - 1
                        tempArr(X, Y).Layer(i).AutoTile = .Tile(X, Y).Layer(i).AutoTile
                        tempArr(X, Y).Layer(i).Tileset = .Tile(X, Y).Layer(i).Tileset
                        tempArr(X, Y).Layer(i).X = .Tile(X, Y).Layer(i).X
                        tempArr(X, Y).Layer(i).Y = .Tile(X, Y).Layer(i).Y
                    Next
                Next
            Next

            x2 = Map.MaxX
            y2 = Map.MaxY
            ' change the data
            .MaxX = nudMaxX.Value
            .MaxY = nudMaxY.Value

            ReDim Map.Tile(.MaxX, .MaxY)
            ReDim Autotile(.MaxX, .MaxY)

            If x2 > .MaxX Then x2 = .MaxX
            If y2 > .MaxY Then y2 = .MaxY

            For X = 0 To .MaxX
                For Y = 0 To .MaxY
                    ReDim Preserve .Tile(X, Y).Layer(LayerType.Count - 1)

                    ReDim Preserve Autotile(X, Y).Layer(LayerType.Count - 1)

                    If X <= x2 Then
                        If Y <= y2 Then
                            .Tile(X, Y).Data1 = tempArr(X, Y).Data1
                            .Tile(X, Y).Data2 = tempArr(X, Y).Data2
                            .Tile(X, Y).Data3 = tempArr(X, Y).Data3
                            .Tile(X, Y).DirBlock = tempArr(X, Y).DirBlock
                            .Tile(X, Y).Type = tempArr(X, Y).Type

                            For i = 1 To LayerType.Count - 1
                                .Tile(X, Y).Layer(i).AutoTile = tempArr(X, Y).Layer(i).AutoTile
                                .Tile(X, Y).Layer(i).Tileset = tempArr(X, Y).Layer(i).Tileset
                                .Tile(X, Y).Layer(i).X = tempArr(X, Y).Layer(i).X
                                .Tile(X, Y).Layer(i).Y = tempArr(X, Y).Layer(i).Y
                            Next
                        End If
                    End If
                Next
            Next
            InitAutotiles()
            ClearTempTile()
            'MapEditorSend()
        End With
        
        rsMap.Width = (Map.MaxX + 1) * Pic_X
        rsMap.Height = (Map.MaxY + 1) * Pic_Y

        GettingMap = False
    End Sub

    Private Sub ChkInstance_CheckedChanged(sender As Object, e As EventArgs) Handles chkInstance.CheckedChanged
        If chkInstance.Checked = True Then
            Map.Instanced = 1
        Else
            Map.Instanced = 0
        End If
    End Sub

    Private Sub BtnMoreOptions_Click(sender As Object, e As EventArgs) Handles btnMoreOptions.Click
        If pnlMoreOptions.Visible = False Then
            pnlMoreOptions.Visible = True
        Else
            pnlMoreOptions.Visible = False
        End If
    End Sub

    Private Sub LstMusic_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstMusic.SelectedIndexChanged
        If lstMusic.SelectedIndex >= 0 Then
            Map.Music = lstMusic.Items(lstMusic.SelectedIndex).ToString
        Else
            Map.Music = ""
        End If
    End Sub

    Private Sub OptDoor_CheckedChanged(sender As Object, e As EventArgs) Handles optDoor.CheckedChanged
        If optDoor.Checked = False Then Exit Sub

        ClearAttributeDialogue()
        pnlAttributes.Visible = True
        fraMapWarp.Visible = True

        scrlMapWarpMap.Maximum = MAX_MAPS
        scrlMapWarpMap.Value = 1
        scrlMapWarpX.Maximum = Byte.MaxValue
        scrlMapWarpY.Maximum = Byte.MaxValue
        scrlMapWarpX.Value = 0
        scrlMapWarpY.Value = 0
    End Sub

#End Region

#Region "Npc's"
    Private Sub LstMapNpc_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstMapNpc.SelectedIndexChanged
        cmbNpcList.SelectedItem = lstMapNpc.SelectedItem
    End Sub

    Private Sub CmbNpcList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbNpcList.SelectedIndexChanged
        If lstMapNpc.SelectedIndex > -1 Then
            If cmbNpcList.SelectedIndex > 0 Then
                lstMapNpc.Items.Item(lstMapNpc.SelectedIndex) = cmbNpcList.SelectedIndex & ": " & Npc(cmbNpcList.SelectedIndex).Name
                Map.Npc(lstMapNpc.SelectedIndex + 1) = cmbNpcList.SelectedIndex
            Else
                lstMapNpc.Items.Item(lstMapNpc.SelectedIndex) = "No NPC"
                Map.Npc(lstMapNpc.SelectedIndex + 1) = 0
            End If

        End If
    End Sub

    Private Sub FrmEditor_MapEditor_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'To Whom It May Concern,
        'This is super shitty code.
        'We know this is super shitty code
        'You will be tempted to remove this code
        'Do not.

        'Find and yell at JC. But leave this code in place
        'Have a nice day
        Me.WindowState = FormWindowState.Maximized
        Application.DoEvents()
        Me.WindowState = FormWindowState.Normal
    End Sub
#End Region

End Class