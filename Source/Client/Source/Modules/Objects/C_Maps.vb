Imports System
Imports System.IO
Imports ASFW
Imports ASFW.IO
Imports System.Drawing

Module C_Maps
#Region "Globals & Types"
    Friend Map As MapRec
    Friend MapLock As New Object()
    Friend MapItem(MAX_MAP_ITEMS) As MapItemRec
    Friend MapNpc(MAX_MAP_NPCS) As MapNpcRec
    Friend TempTile(,) As TempTileRec
#End Region

#Region "DataBase"
    Friend Sub CheckTilesets()
        Dim i As Integer
        i = 1

        While File.Exists(Environment.CurrentDirectory & GfxPath & "\tilesets\" & i & GfxExt)
            NumTileSets = NumTileSets + 1
            i = i + 1
        End While

        If NumTileSets = 0 Then Exit Sub
    End Sub

    Sub ClearMap()

        SyncLock MapLock
            Map.Name = ""
            Map.Tileset = 1
            Map.MaxX = ScreenMapx
            Map.MaxY = ScreenMapy
            Map.BootMap = 0
            Map.BootX = 0
            Map.BootY = 0
            Map.Down = 0
            Map.Left = 0
            Map.Moral = 0
            Map.Music = ""
            Map.Revision = 0
            Map.Right = 0
            Map.Up = 0

            ReDim Map.Npc(MAX_MAP_NPCS)
            ReDim Map.Tile(Map.MaxX, Map.MaxY)

            For x = 0 To ScreenMapx
                For y = 0 To ScreenMapy
                    ReDim Map.Tile(x, y).Layer(LayerType.Count - 1)
                    For l = 0 To LayerType.Count - 1
                        Map.Tile(x, y).Layer(l).Tileset = 0
                        Map.Tile(x, y).Layer(l).X = 0
                        Map.Tile(x, y).Layer(l).Y = 0
                        Map.Tile(x, y).Layer(l).AutoTile = 0
                    Next

                Next
            Next

        End SyncLock

    End Sub

    Sub ClearMapItems()
        Dim i As Integer

        For i = 1 To MAX_MAP_ITEMS
            ClearMapItem(i)
        Next

    End Sub

    Sub ClearMapItem(index As Integer)
        MapItem(index).Frame = 0
        MapItem(index).Num = 0
        MapItem(index).Value = 0
        MapItem(index).X = 0
        MapItem(index).Y = 0
    End Sub

    Sub ClearMapNpc(index As Integer)
        MapNpc(index).Attacking = 0
        MapNpc(index).AttackTimer = 0
        MapNpc(index).Dir = 0
        MapNpc(index).Map = 0
        MapNpc(index).Moving = 0
        MapNpc(index).Num = 0
        MapNpc(index).Steps = 0
        MapNpc(index).Target = 0
        MapNpc(index).TargetType = 0
        MapNpc(index).Vital(VitalType.HP) = 0
        MapNpc(index).Vital(VitalType.MP) = 0
        MapNpc(index).Vital(VitalType.SP) = 0
        MapNpc(index).X = 0
        MapNpc(index).XOffset = 0
        MapNpc(index).Y = 0
        MapNpc(index).YOffset = 0
    End Sub

    Sub ClearMapNpcs()
        Dim i As Integer

        For i = 1 To MAX_MAP_NPCS
            ClearMapNpc(i)
        Next

    End Sub
#End Region

#Region "Incoming Packets"
    Friend Sub Packet_EditMap(ByRef data() As Byte)
        Dim buffer As New ByteStream(data)
        InitMapEditor = True

        buffer.Dispose()
    End Sub

    Sub Packet_CheckMap(ByRef data() As Byte)
        Dim x As Integer, y As Integer, i As Integer
        Dim needMap As Byte
        Dim buffer As New ByteStream(data)
        GettingMap = True

        ' Erase all players except self
        For i = 1 To TotalOnline 'MAX_PLAYERS
            If i <> Myindex Then
                SetPlayerMap(i, 0)
            End If
        Next

        ' Erase all temporary tile values
        ClearTempTile()
        ClearMapNpcs()
        ClearMapItems()
        ClearBlood()
        ClearMap()

        ' Get map num
        x = buffer.ReadInt32
        ' Get revision
        y = buffer.ReadInt32

        needMap = 1

        ' Either the revisions didn't match or we dont have the map, so we need it
        buffer = New ByteStream(4)
        buffer.WriteInt32(ClientPacket.CNeedMap)
        buffer.WriteInt32(needMap)
        Network.SendData(buffer.ToPacket)

        buffer.Dispose()
    End Sub

    Sub Packet_MapData(ByRef data() As Byte)
        Dim x As Integer, y As Integer, i As Integer, mapNum As Integer
        Dim buffer As New ByteStream(Compression.DecompressBytes(data))

        MapData = False

        ClearMap()

        SyncLock MapLock
            If buffer.ReadInt32 = 1 Then

                mapNum = buffer.ReadInt32
                Map.Name = buffer.ReadString.Trim
                Map.Music = buffer.ReadString.Trim
                Map.Revision = buffer.ReadInt32
                Map.Moral = buffer.ReadInt32
                Map.Tileset = buffer.ReadInt32
                Map.Up = buffer.ReadInt32
                Map.Down = buffer.ReadInt32
                Map.Left = buffer.ReadInt32
                Map.Right = buffer.ReadInt32
                Map.BootMap = buffer.ReadInt32
                Map.BootX = buffer.ReadInt32
                Map.BootY = buffer.ReadInt32
                Map.MaxX = buffer.ReadInt32
                Map.MaxY = buffer.ReadInt32
                Map.WeatherType = buffer.ReadInt32
                Map.Fogindex = buffer.ReadInt32
                Map.WeatherIntensity = buffer.ReadInt32
                Map.FogAlpha = buffer.ReadInt32
                Map.FogSpeed = buffer.ReadInt32
                Map.HasMapTint = buffer.ReadInt32
                Map.MapTintR = buffer.ReadInt32
                Map.MapTintG = buffer.ReadInt32
                Map.MapTintB = buffer.ReadInt32
                Map.MapTintA = buffer.ReadInt32
                Map.Instanced = buffer.ReadInt32
                Map.Panorama = buffer.ReadInt32
                Map.Parallax = buffer.ReadInt32

                ReDim Map.Tile(Map.MaxX, Map.MaxY)

                For x = 1 To MAX_MAP_NPCS
                    Map.Npc(x) = buffer.ReadInt32
                Next

                For x = 0 To Map.MaxX
                    For y = 0 To Map.MaxY
                        Map.Tile(x, y).Data1 = buffer.ReadInt32
                        Map.Tile(x, y).Data2 = buffer.ReadInt32
                        Map.Tile(x, y).Data3 = buffer.ReadInt32
                        Map.Tile(x, y).DirBlock = buffer.ReadInt32

                        ReDim Map.Tile(x, y).Layer(LayerType.Count - 1)

                        For i = 0 To LayerType.Count - 1
                            Map.Tile(x, y).Layer(i).Tileset = buffer.ReadInt32
                            Map.Tile(x, y).Layer(i).X = buffer.ReadInt32
                            Map.Tile(x, y).Layer(i).Y = buffer.ReadInt32
                            Map.Tile(x, y).Layer(i).AutoTile = buffer.ReadInt32
                        Next
                        Map.Tile(x, y).Type = buffer.ReadInt32
                    Next
                Next

                'Event Data!
                ResetEventdata()

                Map.EventCount = buffer.ReadInt32

                If Map.EventCount > 0 Then
                    ReDim Map.Events(Map.EventCount)
                    For i = 1 To Map.EventCount
                        With Map.Events(i)
                            .Name = buffer.ReadString.Trim
                            .Globals = buffer.ReadInt32
                            .X = buffer.ReadInt32
                            .Y = buffer.ReadInt32
                            .PageCount = buffer.ReadInt32
                        End With
                        If Map.Events(i).PageCount > 0 Then
                            ReDim Map.Events(i).Pages(Map.Events(i).PageCount)
                            For x = 1 To Map.Events(i).PageCount
                                With Map.Events(i).Pages(x)
                                    .ChkVariable = buffer.ReadInt32
                                    .Variableindex = buffer.ReadInt32
                                    .VariableCondition = buffer.ReadInt32
                                    .VariableCompare = buffer.ReadInt32

                                    .ChkSwitch = buffer.ReadInt32
                                    .Switchindex = buffer.ReadInt32
                                    .SwitchCompare = buffer.ReadInt32

                                    .ChkHasItem = buffer.ReadInt32
                                    .HasItemindex = buffer.ReadInt32
                                    .HasItemAmount = buffer.ReadInt32

                                    .ChkSelfSwitch = buffer.ReadInt32
                                    .SelfSwitchindex = buffer.ReadInt32
                                    .SelfSwitchCompare = buffer.ReadInt32

                                    .GraphicType = buffer.ReadInt32
                                    .Graphic = buffer.ReadInt32
                                    .GraphicX = buffer.ReadInt32
                                    .GraphicY = buffer.ReadInt32
                                    .GraphicX2 = buffer.ReadInt32
                                    .GraphicY2 = buffer.ReadInt32

                                    .MoveType = buffer.ReadInt32
                                    .MoveSpeed = buffer.ReadInt32
                                    .MoveFreq = buffer.ReadInt32

                                    .MoveRouteCount = buffer.ReadInt32

                                    .IgnoreMoveRoute = buffer.ReadInt32
                                    .RepeatMoveRoute = buffer.ReadInt32

                                    If .MoveRouteCount > 0 Then
                                        ReDim Map.Events(i).Pages(x).MoveRoute(.MoveRouteCount)
                                        For y = 1 To .MoveRouteCount
                                            .MoveRoute(y).Index = buffer.ReadInt32
                                            .MoveRoute(y).Data1 = buffer.ReadInt32
                                            .MoveRoute(y).Data2 = buffer.ReadInt32
                                            .MoveRoute(y).Data3 = buffer.ReadInt32
                                            .MoveRoute(y).Data4 = buffer.ReadInt32
                                            .MoveRoute(y).Data5 = buffer.ReadInt32
                                            .MoveRoute(y).Data6 = buffer.ReadInt32
                                        Next
                                    End If

                                    .WalkAnim = buffer.ReadInt32
                                    .DirFix = buffer.ReadInt32
                                    .WalkThrough = buffer.ReadInt32
                                    .ShowName = buffer.ReadInt32
                                    .Trigger = buffer.ReadInt32
                                    .CommandListCount = buffer.ReadInt32

                                    .Position = buffer.ReadInt32
                                    .Questnum = buffer.ReadInt32

                                    .ChkPlayerGender = buffer.ReadInt32
                                End With

                                If Map.Events(i).Pages(x).CommandListCount > 0 Then
                                    ReDim Map.Events(i).Pages(x).CommandList(Map.Events(i).Pages(x).CommandListCount)
                                    For y = 1 To Map.Events(i).Pages(x).CommandListCount
                                        Map.Events(i).Pages(x).CommandList(y).CommandCount = buffer.ReadInt32
                                        Map.Events(i).Pages(x).CommandList(y).ParentList = buffer.ReadInt32
                                        If Map.Events(i).Pages(x).CommandList(y).CommandCount > 0 Then
                                            ReDim Map.Events(i).Pages(x).CommandList(y).Commands(Map.Events(i).Pages(x).CommandList(y).CommandCount)
                                            For z = 1 To Map.Events(i).Pages(x).CommandList(y).CommandCount
                                                With Map.Events(i).Pages(x).CommandList(y).Commands(z)
                                                    .Index = buffer.ReadInt32
                                                    .Text1 = buffer.ReadString.Trim
                                                    .Text2 = buffer.ReadString.Trim
                                                    .Text3 = buffer.ReadString.Trim
                                                    .Text4 = buffer.ReadString.Trim
                                                    .Text5 = buffer.ReadString.Trim
                                                    .Data1 = buffer.ReadInt32
                                                    .Data2 = buffer.ReadInt32
                                                    .Data3 = buffer.ReadInt32
                                                    .Data4 = buffer.ReadInt32
                                                    .Data5 = buffer.ReadInt32
                                                    .Data6 = buffer.ReadInt32
                                                    .ConditionalBranch.CommandList = buffer.ReadInt32
                                                    .ConditionalBranch.Condition = buffer.ReadInt32
                                                    .ConditionalBranch.Data1 = buffer.ReadInt32
                                                    .ConditionalBranch.Data2 = buffer.ReadInt32
                                                    .ConditionalBranch.Data3 = buffer.ReadInt32
                                                    .ConditionalBranch.ElseCommandList = buffer.ReadInt32
                                                    .MoveRouteCount = buffer.ReadInt32
                                                    If .MoveRouteCount > 0 Then
                                                        ReDim Preserve .MoveRoute(.MoveRouteCount)
                                                        For w = 1 To .MoveRouteCount
                                                            .MoveRoute(w).Index = buffer.ReadInt32
                                                            .MoveRoute(w).Data1 = buffer.ReadInt32
                                                            .MoveRoute(w).Data2 = buffer.ReadInt32
                                                            .MoveRoute(w).Data3 = buffer.ReadInt32
                                                            .MoveRoute(w).Data4 = buffer.ReadInt32
                                                            .MoveRoute(w).Data5 = buffer.ReadInt32
                                                            .MoveRoute(w).Data6 = buffer.ReadInt32
                                                        Next
                                                    End If
                                                End With
                                            Next
                                        End If
                                    Next
                                End If
                            Next
                        End If
                    Next
                End If
                'End Event Data
            End If

            For i = 1 To MAX_MAP_ITEMS
                MapItem(i).Num = buffer.ReadInt32
                MapItem(i).Value = buffer.ReadInt32()
                MapItem(i).X = buffer.ReadInt32()
                MapItem(i).Y = buffer.ReadInt32()
            Next

            For i = 1 To MAX_MAP_NPCS
                MapNpc(i).Num = buffer.ReadInt32()
                MapNpc(i).X = buffer.ReadInt32()
                MapNpc(i).Y = buffer.ReadInt32()
                MapNpc(i).Dir = buffer.ReadInt32()
                MapNpc(i).Vital(VitalType.HP) = buffer.ReadInt32()
                MapNpc(i).Vital(VitalType.MP) = buffer.ReadInt32()
            Next

            If buffer.ReadInt32 = 1 Then
                ResourceIndex = buffer.ReadInt32
                ResourcesInit = False

                If ResourceIndex > 0 Then
                    ReDim MapResource(ResourceIndex)

                    For i = 0 To ResourceIndex
                        MapResource(i).ResourceState = buffer.ReadInt32
                        MapResource(i).X = buffer.ReadInt32
                        MapResource(i).Y = buffer.ReadInt32
                    Next

                    ResourcesInit = True
                Else
                    ReDim MapResource(1)
                End If
            End If

            ClearTempTile()

            buffer.Dispose()

        End SyncLock

        InitAutotiles()

        MapData = True

        For i = 1 To Byte.MaxValue
            ClearActionMsg(i)
        Next

        CurrentWeather = Map.WeatherType
        CurrentWeatherIntensity = Map.WeatherIntensity
        CurrentFog = Map.Fogindex
        CurrentFogSpeed = Map.FogSpeed
        CurrentFogOpacity = Map.FogAlpha
        CurrentTintR = Map.MapTintR
        CurrentTintG = Map.MapTintG
        CurrentTintB = Map.MapTintB
        CurrentTintA = Map.MapTintA

        PlayMusic(Map.Music.Trim)

        UpdateDrawMapName()

        GettingMap = False
        CanMoveNow = True

    End Sub

    Sub Packet_MapNPCData(ByRef data() As Byte)
        Dim i As Integer
        Dim buffer As New ByteStream(data)
        For i = 1 To MAX_MAP_NPCS

            With MapNpc(i)
                .Num = buffer.ReadInt32
                .X = buffer.ReadInt32
                .Y = buffer.ReadInt32
                .Dir = buffer.ReadInt32
                .Vital(VitalType.HP) = buffer.ReadInt32
                .Vital(VitalType.MP) = buffer.ReadInt32
            End With

        Next

        buffer.Dispose()
    End Sub

    Sub Packet_MapNPCUpdate(ByRef data() As Byte)
        Dim npcNum As Integer
        Dim buffer As New ByteStream(data)
        npcNum = buffer.ReadInt32

        With MapNpc(npcNum)
            .Num = buffer.ReadInt32
            .X = buffer.ReadInt32
            .Y = buffer.ReadInt32
            .Dir = buffer.ReadInt32
            .Vital(VitalType.HP) = buffer.ReadInt32
            .Vital(VitalType.MP) = buffer.ReadInt32
        End With

        buffer.Dispose()
    End Sub

    Sub Packet_MapDone(ByRef data() As Byte)
        Dim i As Integer
        Dim musicFile As String

        For i = 1 To Byte.MaxValue
            ClearActionMsg(i)
        Next

        CurrentWeather = Map.WeatherType
        CurrentWeatherIntensity = Map.WeatherIntensity
        CurrentFog = Map.Fogindex
        CurrentFogSpeed = Map.FogSpeed
        CurrentFogOpacity = Map.FogAlpha
        CurrentTintR = Map.MapTintR
        CurrentTintG = Map.MapTintG
        CurrentTintB = Map.MapTintB
        CurrentTintA = Map.MapTintA

        musicFile = Map.Music.Trim
        PlayMusic(musicFile)

        UpdateDrawMapName()

        GettingMap = False
        CanMoveNow = True

    End Sub
#End Region

#Region "Outgoing Packets"
    Friend Sub SendPlayerRequestNewMap()
        If GettingMap Then Exit Sub

        Dim buffer As New ByteStream(4)

        buffer.WriteInt32(ClientPacket.CRequestNewMap)
        buffer.WriteInt32(GetPlayerDir(Myindex))

        Network.SendData(buffer.ToPacket)
        buffer.Dispose()

    End Sub

    Friend Sub SendRequestEditMap()
        Dim buffer As New ByteStream(4)

        buffer.WriteInt32(ClientPacket.CRequestEditMap)
        Network.SendData(buffer.ToPacket)

        buffer.Dispose()
    End Sub

    Friend Sub SendMap()
        Dim x As Integer, y As Integer, i As Integer
        Dim data() As Byte
        Dim buffer As New ByteStream(4)
        CanMoveNow = False

        buffer.WriteString((Map.Name.Trim))
        buffer.WriteString((Map.Music.Trim))
        buffer.WriteInt32(Map.Moral)
        buffer.WriteInt32(Map.Tileset)
        buffer.WriteInt32(Map.Up)
        buffer.WriteInt32(Map.Down)
        buffer.WriteInt32(Map.Left)
        buffer.WriteInt32(Map.Right)
        buffer.WriteInt32(Map.BootMap)
        buffer.WriteInt32(Map.BootX)
        buffer.WriteInt32(Map.BootY)
        buffer.WriteInt32(Map.MaxX)
        buffer.WriteInt32(Map.MaxY)
        buffer.WriteInt32(Map.WeatherType)
        buffer.WriteInt32(Map.Fogindex)
        buffer.WriteInt32(Map.WeatherIntensity)
        buffer.WriteInt32(Map.FogAlpha)
        buffer.WriteInt32(Map.FogSpeed)
        buffer.WriteInt32(Map.HasMapTint)
        buffer.WriteInt32(Map.MapTintR)
        buffer.WriteInt32(Map.MapTintG)
        buffer.WriteInt32(Map.MapTintB)
        buffer.WriteInt32(Map.MapTintA)
        buffer.WriteInt32(Map.Instanced)
        buffer.WriteInt32(Map.Panorama)
        buffer.WriteInt32(Map.Parallax)

        For i = 1 To MAX_MAP_NPCS
            buffer.WriteInt32(Map.Npc(i))
        Next

        For x = 0 To Map.MaxX
            For y = 0 To Map.MaxY
                buffer.WriteInt32(Map.Tile(x, y).Data1)
                buffer.WriteInt32(Map.Tile(x, y).Data2)
                buffer.WriteInt32(Map.Tile(x, y).Data3)
                buffer.WriteInt32(Map.Tile(x, y).DirBlock)
                For i = 0 To LayerType.Count - 1
                    buffer.WriteInt32(Map.Tile(x, y).Layer(i).Tileset)
                    buffer.WriteInt32(Map.Tile(x, y).Layer(i).X)
                    buffer.WriteInt32(Map.Tile(x, y).Layer(i).Y)
                    buffer.WriteInt32(Map.Tile(x, y).Layer(i).AutoTile)
                Next
                buffer.WriteInt32(Map.Tile(x, y).Type)
            Next
        Next

        'Event Data
        buffer.WriteInt32(Map.EventCount)
        If Map.EventCount > 0 Then
            For i = 1 To Map.EventCount
                With Map.Events(i)
                    buffer.WriteString((.Name.Trim))
                    buffer.WriteInt32(.Globals)
                    buffer.WriteInt32(.X)
                    buffer.WriteInt32(.Y)
                    buffer.WriteInt32(.PageCount)
                End With
                If Map.Events(i).PageCount > 0 Then
                    For x = 1 To Map.Events(i).PageCount
                        With Map.Events(i).Pages(x)
                            buffer.WriteInt32(.ChkVariable)
                            buffer.WriteInt32(.Variableindex)
                            buffer.WriteInt32(.VariableCondition)
                            buffer.WriteInt32(.VariableCompare)
                            buffer.WriteInt32(.ChkSwitch)
                            buffer.WriteInt32(.Switchindex)
                            buffer.WriteInt32(.SwitchCompare)
                            buffer.WriteInt32(.ChkHasItem)
                            buffer.WriteInt32(.HasItemindex)
                            buffer.WriteInt32(.HasItemAmount)
                            buffer.WriteInt32(.ChkSelfSwitch)
                            buffer.WriteInt32(.SelfSwitchindex)
                            buffer.WriteInt32(.SelfSwitchCompare)
                            buffer.WriteInt32(.GraphicType)
                            buffer.WriteInt32(.Graphic)
                            buffer.WriteInt32(.GraphicX)
                            buffer.WriteInt32(.GraphicY)
                            buffer.WriteInt32(.GraphicX2)
                            buffer.WriteInt32(.GraphicY2)
                            buffer.WriteInt32(.MoveType)
                            buffer.WriteInt32(.MoveSpeed)
                            buffer.WriteInt32(.MoveFreq)
                            buffer.WriteInt32(Map.Events(i).Pages(x).MoveRouteCount)
                            buffer.WriteInt32(.IgnoreMoveRoute)
                            buffer.WriteInt32(.RepeatMoveRoute)
                            If .MoveRouteCount > 0 Then
                                For y = 1 To .MoveRouteCount
                                    buffer.WriteInt32(.MoveRoute(y).Index)
                                    buffer.WriteInt32(.MoveRoute(y).Data1)
                                    buffer.WriteInt32(.MoveRoute(y).Data2)
                                    buffer.WriteInt32(.MoveRoute(y).Data3)
                                    buffer.WriteInt32(.MoveRoute(y).Data4)
                                    buffer.WriteInt32(.MoveRoute(y).Data5)
                                    buffer.WriteInt32(.MoveRoute(y).Data6)
                                Next
                            End If
                            buffer.WriteInt32(.WalkAnim)
                            buffer.WriteInt32(.DirFix)
                            buffer.WriteInt32(.WalkThrough)
                            buffer.WriteInt32(.ShowName)
                            buffer.WriteInt32(.Trigger)
                            buffer.WriteInt32(.CommandListCount)
                            buffer.WriteInt32(.Position)
                            buffer.WriteInt32(.Questnum)

                            buffer.WriteInt32(.ChkPlayerGender)
                        End With
                        If Map.Events(i).Pages(x).CommandListCount > 0 Then
                            For y = 1 To Map.Events(i).Pages(x).CommandListCount
                                buffer.WriteInt32(Map.Events(i).Pages(x).CommandList(y).CommandCount)
                                buffer.WriteInt32(Map.Events(i).Pages(x).CommandList(y).ParentList)
                                If Map.Events(i).Pages(x).CommandList(y).CommandCount > 0 Then
                                    For z = 1 To Map.Events(i).Pages(x).CommandList(y).CommandCount
                                        With Map.Events(i).Pages(x).CommandList(y).Commands(z)
                                            buffer.WriteInt32(.Index)
                                            buffer.WriteString((.Text1.Trim))
                                            buffer.WriteString((.Text2.Trim))
                                            buffer.WriteString((.Text3.Trim))
                                            buffer.WriteString((.Text4.Trim))
                                            buffer.WriteString((.Text5.Trim))
                                            buffer.WriteInt32(.Data1)
                                            buffer.WriteInt32(.Data2)
                                            buffer.WriteInt32(.Data3)
                                            buffer.WriteInt32(.Data4)
                                            buffer.WriteInt32(.Data5)
                                            buffer.WriteInt32(.Data6)
                                            buffer.WriteInt32(.ConditionalBranch.CommandList)
                                            buffer.WriteInt32(.ConditionalBranch.Condition)
                                            buffer.WriteInt32(.ConditionalBranch.Data1)
                                            buffer.WriteInt32(.ConditionalBranch.Data2)
                                            buffer.WriteInt32(.ConditionalBranch.Data3)
                                            buffer.WriteInt32(.ConditionalBranch.ElseCommandList)
                                            buffer.WriteInt32(.MoveRouteCount)
                                            If .MoveRouteCount > 0 Then
                                                For w = 1 To .MoveRouteCount
                                                    buffer.WriteInt32(.MoveRoute(w).Index)
                                                    buffer.WriteInt32(.MoveRoute(w).Data1)
                                                    buffer.WriteInt32(.MoveRoute(w).Data2)
                                                    buffer.WriteInt32(.MoveRoute(w).Data3)
                                                    buffer.WriteInt32(.MoveRoute(w).Data4)
                                                    buffer.WriteInt32(.MoveRoute(w).Data5)
                                                    buffer.WriteInt32(.MoveRoute(w).Data6)
                                                Next
                                            End If
                                        End With
                                    Next
                                End If
                            Next
                        End If
                    Next
                End If
            Next
        End If
        'End Event Data

        data = buffer.ToArray

        buffer = New ByteStream(4)
        buffer.WriteInt32(ClientPacket.CSaveMap)
        buffer.WriteBlock(Compression.CompressBytes(data))

        Network.SendData(buffer.ToPacket)
        buffer.Dispose()
    End Sub

    Friend Sub SendMapRespawn()
        Dim buffer As New ByteStream(4)

        buffer.WriteInt32(ClientPacket.CMapRespawn)

        Network.SendData(buffer.ToPacket)
        buffer.Dispose()
    End Sub


#End Region

#Region "Drawing"
    Friend Sub DrawMapTile(x As Integer, y As Integer)
        Dim i As Integer
        Dim srcrect As New Rectangle(0, 0, 0, 0)

        If GettingMap Then Exit Sub
        If Map.Tile Is Nothing Then Exit Sub
        If MapData = False Then Exit Sub

        For i = LayerType.Ground To LayerType.Mask2
            If Map.Tile(x, y).Layer Is Nothing Then Exit Sub
            ' skip tile if tileset isn't set
            If Map.Tile(x, y).Layer(i).Tileset > 0 AndAlso Map.Tile(x, y).Layer(i).Tileset <= NumTileSets Then
                If TileSetTextureInfo(Map.Tile(x, y).Layer(i).Tileset).IsLoaded = False Then
                    LoadTexture(Map.Tile(x, y).Layer(i).Tileset, 1)
                End If
                ' we use it, lets update timer
                With TileSetTextureInfo(Map.Tile(x, y).Layer(i).Tileset)
                    .TextureTimer = GetTickCount() + 100000
                End With
                If Autotile(x, y).Layer(i).RenderState = RenderStateNormal Then
                    With srcrect
                        .X = Map.Tile(x, y).Layer(i).X * 32
                        .Y = Map.Tile(x, y).Layer(i).Y * 32
                        .Width = 32
                        .Height = 32
                    End With

                    RenderSprite(TileSetSprite(Map.Tile(x, y).Layer(i).Tileset), GameWindow, ConvertMapX(x * PicX), ConvertMapY(y * PicY), srcrect.X, srcrect.Y, srcrect.Width, srcrect.Height)

                ElseIf Autotile(x, y).Layer(i).RenderState = RenderStateAutotile Then
                    ' Draw autotiles
                    DrawAutoTile(i, ConvertMapX(x * PicX), ConvertMapY(y * PicY), 1, x, y, 0, False)
                    DrawAutoTile(i, ConvertMapX(x * PicX) + 16, ConvertMapY(y * PicY), 2, x, y, 0, False)
                    DrawAutoTile(i, ConvertMapX(x * PicX), ConvertMapY(y * PicY) + 16, 3, x, y, 0, False)
                    DrawAutoTile(i, ConvertMapX(x * PicX) + 16, ConvertMapY(y * PicY) + 16, 4, x, y, 0, False)
                End If
            End If
        Next

    End Sub

    Friend Sub DrawMapFringeTile(x As Integer, y As Integer)
        Dim i As Integer
        Dim srcrect As New Rectangle(0, 0, 0, 0)
        'Dim dest As Rectangle = New Rectangle(FrmMainGame.PointToScreen(FrmMainGame.picscreen.Location), New Size(32, 32))

        If GettingMap Then Exit Sub
        If Map.Tile Is Nothing Then Exit Sub
        If MapData = False Then Exit Sub

        For i = LayerType.Fringe To LayerType.Fringe2
            If Map.Tile(x, y).Layer Is Nothing Then Exit Sub
            ' skip tile if tileset isn't set
            If Map.Tile(x, y).Layer(i).Tileset > 0 AndAlso Map.Tile(x, y).Layer(i).Tileset <= NumTileSets Then
                If TileSetTextureInfo(Map.Tile(x, y).Layer(i).Tileset).IsLoaded = False Then
                    LoadTexture(Map.Tile(x, y).Layer(i).Tileset, 1)
                End If

                ' we use it, lets update timer
                With TileSetTextureInfo(Map.Tile(x, y).Layer(i).Tileset)
                    .TextureTimer = GetTickCount() + 100000
                End With

                ' render
                If Autotile(x, y).Layer(i).RenderState = RenderStateNormal Then
                    With srcrect
                        .X = Map.Tile(x, y).Layer(i).X * 32
                        .Y = Map.Tile(x, y).Layer(i).Y * 32
                        .Width = 32
                        .Height = 32
                    End With

                    RenderSprite(TileSetSprite(Map.Tile(x, y).Layer(i).Tileset), GameWindow, ConvertMapX(x * PicX), ConvertMapY(y * PicY), srcrect.X, srcrect.Y, srcrect.Width, srcrect.Height)

                ElseIf Autotile(x, y).Layer(i).RenderState = RenderStateAutotile Then
                    ' Draw autotiles
                    DrawAutoTile(i, ConvertMapX(x * PicX), ConvertMapY(y * PicY), 1, x, y, 0, False)
                    DrawAutoTile(i, ConvertMapX(x * PicX) + 16, ConvertMapY(y * PicY), 2, x, y, 0, False)
                    DrawAutoTile(i, ConvertMapX(x * PicX), ConvertMapY(y * PicY) + 16, 3, x, y, 0, False)
                    DrawAutoTile(i, ConvertMapX(x * PicX) + 16, ConvertMapY(y * PicY) + 16, 4, x, y, 0, False)
                End If
            End If
        Next

    End Sub
#End Region

End Module





















































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































