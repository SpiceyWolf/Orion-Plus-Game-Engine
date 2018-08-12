Imports System.Windows.Forms
Imports ASFW
Imports ASFW.IO

Namespace Network
    Partial Friend Module modNetwork
        Friend Sub SendNewAccount(name As String, password As String)
            Dim buffer As New ByteStream(4)

            buffer.WriteInt32(ClientPacket.CNewAccount)
            buffer.WriteString((Configuration.Encryption.EncryptString(name)))
            buffer.WriteString((Configuration.Encryption.EncryptString(password)))
            Socket.SendData(buffer.ToPacket)

            buffer.Dispose()
        End Sub

        Friend Sub SendAddChar(slot As Integer, name As String, sex As Integer, classNum As Integer, sprite As Integer)
            Dim buffer As New ByteStream(4)

            buffer.WriteInt32(ClientPacket.CAddChar)
            buffer.WriteInt32(slot)
            buffer.WriteString((name))
            buffer.WriteInt32(sex)
            buffer.WriteInt32(classNum)
            buffer.WriteInt32(sprite)
            Socket.SendData(buffer.ToPacket)

            buffer.Dispose()
        End Sub

        Friend Sub SendLogin(name As String, password As String)
            Dim buffer As New ByteStream(4)

            buffer.WriteInt32(ClientPacket.CLogin)
            buffer.WriteString(Configuration.Encryption.EncryptString(name))
            buffer.WriteString(Configuration.Encryption.EncryptString(password))
            buffer.WriteString(Configuration.Encryption.EncryptString(Application.ProductVersion))
            Socket.SendData(buffer.ToPacket)

            buffer.Dispose()
        End Sub

        Sub GetPing()
            Dim buffer As New ByteStream(4)
            PingStart = GetTickCount()

            buffer.WriteInt32(ClientPacket.CCheckPing)
            Socket.SendData(buffer.ToPacket)

            buffer.Dispose()
        End Sub

        Friend Sub SendPlayerMove()
            Dim buffer As New ByteStream(4)

            buffer.WriteInt32(ClientPacket.CPlayerMove)
            buffer.WriteInt32(GetPlayerDir(Myindex))
            buffer.WriteInt32(Player(Myindex).Moving)
            buffer.WriteInt32(Player(Myindex).X)
            buffer.WriteInt32(Player(Myindex).Y)

            Socket.SendData(buffer.ToPacket)
            buffer.Dispose()
        End Sub

        Friend Sub SayMsg(text As String)
            Dim buffer As New ByteStream(4)

            buffer.WriteInt32(ClientPacket.CSayMsg)
            buffer.WriteString((text))

            Socket.SendData(buffer.ToPacket)
            buffer.Dispose()
        End Sub

        Friend Sub SendKick(name As String)
            Dim buffer As New ByteStream(4)

            buffer.WriteInt32(ClientPacket.CKickPlayer)
            buffer.WriteString((name))

            Socket.SendData(buffer.ToPacket)
            buffer.Dispose()
        End Sub

        Friend Sub SendBan(name As String)
            Dim buffer As New ByteStream(4)

            buffer.WriteInt32(ClientPacket.CBanPlayer)
            buffer.WriteString((name))

            Socket.SendData(buffer.ToPacket)
            buffer.Dispose()
        End Sub

        Friend Sub WarpMeTo(name As String)
            Dim buffer As New ByteStream(4)

            buffer.WriteInt32(ClientPacket.CWarpMeTo)
            buffer.WriteString((name))

            Socket.SendData(buffer.ToPacket)
            buffer.Dispose()
        End Sub

        Friend Sub WarpToMe(name As String)
            Dim buffer As New ByteStream(4)

            buffer.WriteInt32(ClientPacket.CWarpToMe)
            buffer.WriteString((name))

            Socket.SendData(buffer.ToPacket)
            buffer.Dispose()
        End Sub

        Friend Sub WarpTo(mapNum As Integer)
            Dim buffer As New ByteStream(4)

            buffer.WriteInt32(ClientPacket.CWarpTo)
            buffer.WriteInt32(mapNum)

            Socket.SendData(buffer.ToPacket)
            buffer.Dispose()
        End Sub

        Friend Sub SendRequestLevelUp()
            Dim buffer As New ByteStream(4)

            buffer.WriteInt32(ClientPacket.CRequestLevelUp)

            Socket.SendData(buffer.ToPacket)
            buffer.Dispose()
        End Sub

        Sub SendSpawnItem(tmpItem As Integer, tmpAmount As Integer)
            Dim buffer As New ByteStream(4)

            buffer.WriteInt32(ClientPacket.CSpawnItem)
            buffer.WriteInt32(tmpItem)
            buffer.WriteInt32(tmpAmount)

            Socket.SendData(buffer.ToPacket)
            buffer.Dispose()
        End Sub

        Friend Sub SendSetSprite(spriteNum As Integer)
            Dim buffer As New ByteStream(4)

            buffer.WriteInt32(ClientPacket.CSetSprite)
            buffer.WriteInt32(spriteNum)

            Socket.SendData(buffer.ToPacket)
            buffer.Dispose()
        End Sub

        Friend Sub SendSetAccess(name As String, access As Byte)
            Dim buffer As New ByteStream(4)

            buffer.WriteInt32(ClientPacket.CSetAccess)
            buffer.WriteString((name))
            buffer.WriteInt32(access)

            Socket.SendData(buffer.ToPacket)
            buffer.Dispose()
        End Sub

        Sub SendAttack()
            Dim buffer As New ByteStream(4)

            buffer.WriteInt32(ClientPacket.CAttack)

            Socket.SendData(buffer.ToPacket)
            buffer.Dispose()
        End Sub

        Friend Sub SendPlayerDir()
            Dim buffer As New ByteStream(4)

            buffer.WriteInt32(ClientPacket.CPlayerDir)
            buffer.WriteInt32(GetPlayerDir(Myindex))

            Socket.SendData(buffer.ToPacket)
            buffer.Dispose()
        End Sub

        Sub SendRequestNpcs()
            Dim buffer As New ByteStream(4)

            buffer.WriteInt32(ClientPacket.CRequestNPCS)

            Socket.SendData(buffer.ToPacket)
            buffer.Dispose()
        End Sub

        Sub SendRequestSkills()
            Dim buffer As New ByteStream(4)

            buffer.WriteInt32(ClientPacket.CRequestSkills)

            Socket.SendData(buffer.ToPacket)
            buffer.Dispose()
        End Sub



        Sub SendRequestAnimations()
            Dim buffer As New ByteStream(4)

            buffer.WriteInt32(ClientPacket.CRequestAnimations)

            Socket.SendData(buffer.ToPacket)
            buffer.Dispose()
        End Sub

        Sub SendTrainStat(statNum As Byte)
            Dim buffer As New ByteStream(4)

            buffer.WriteInt32(ClientPacket.CTrainStat)
            buffer.WriteInt32(statNum)

            Socket.SendData(buffer.ToPacket)
            buffer.Dispose()
        End Sub

        Sub SendRequestPlayerData()
            Dim buffer As New ByteStream(4)

            buffer.WriteInt32(ClientPacket.CRequestPlayerData)

            Socket.SendData(buffer.ToPacket)
            buffer.Dispose()
        End Sub

        Friend Sub BroadcastMsg(text As String)
            Dim buffer As New ByteStream(4)

            buffer.WriteInt32(ClientPacket.CBroadcastMsg)
            buffer.WriteString(text.Trim)

            Socket.SendData(buffer.ToPacket)
            buffer.Dispose()
        End Sub

        Friend Sub PlayerMsg(text As String, msgTo As String)
            Dim buffer As New ByteStream(4)

            buffer.WriteInt32(ClientPacket.CPlayerMsg)
            buffer.WriteString((msgTo))
            buffer.WriteString((text))

            Socket.SendData(buffer.ToPacket)
            buffer.Dispose()
        End Sub

        Friend Sub SendWhosOnline()
            Dim buffer As New ByteStream(4)

            buffer.WriteInt32(ClientPacket.CWhosOnline)

            Socket.SendData(buffer.ToPacket)
            buffer.Dispose()
        End Sub

        Friend Sub SendMotdChange(motd As String)
            Dim buffer As New ByteStream(4)

            buffer.WriteInt32(ClientPacket.CSetMotd)
            buffer.WriteString((motd))

            Socket.SendData(buffer.ToPacket)
            buffer.Dispose()
        End Sub

        Friend Sub SendBanList()
            Dim buffer As New ByteStream(4)

            buffer.WriteInt32(ClientPacket.CBanList)

            Socket.SendData(buffer.ToPacket)
            buffer.Dispose()
        End Sub

        Sub SendChangeInvSlots(oldSlot As Integer, newSlot As Integer)
            Dim buffer As New ByteStream(4)

            buffer.WriteInt32(ClientPacket.CSwapInvSlots)
            buffer.WriteInt32(oldSlot)
            buffer.WriteInt32(newSlot)

            Socket.SendData(buffer.ToPacket)
            buffer.Dispose()
        End Sub

        Friend Sub SendUseItem(invNum As Integer)
            Dim buffer As New ByteStream(4)

            buffer.WriteInt32(ClientPacket.CUseItem)
            buffer.WriteInt32(invNum)

            Socket.SendData(buffer.ToPacket)
            buffer.Dispose()
        End Sub

        Friend Sub SendDropItem(invNum As Integer, amount As Integer)
            Dim buffer As New ByteStream(4)

            If InBank OrElse InShop Then Exit Sub

            ' do basic checks
            If invNum < 1 OrElse invNum > MAX_INV Then Exit Sub
            If PlayerInv(invNum).Num < 1 OrElse PlayerInv(invNum).Num > MAX_ITEMS Then Exit Sub
            If Item(GetPlayerInvItemNum(Myindex, invNum)).Type = ItemType.Currency OrElse Item(GetPlayerInvItemNum(Myindex, invNum)).Stackable = 1 Then
                If amount < 1 OrElse amount > PlayerInv(invNum).Value Then Exit Sub
            End If

            buffer.WriteInt32(ClientPacket.CMapDropItem)
            buffer.WriteInt32(invNum)
            buffer.WriteInt32(amount)

            Socket.SendData(buffer.ToPacket)
            buffer.Dispose()
        End Sub



        Sub PlayerSearch(curX As Integer, curY As Integer, rClick As Byte)
            Dim buffer As New ByteStream(4)

            If IsInBounds() Then
                buffer.WriteInt32(ClientPacket.CSearch)
                buffer.WriteInt32(curX)
                buffer.WriteInt32(curY)
                buffer.WriteInt32(rClick)
                Socket.SendData(buffer.ToPacket)
            End If

            buffer.Dispose()
        End Sub

        Friend Sub AdminWarp(x As Integer, y As Integer)
            Dim buffer As New ByteStream(4)

            buffer.WriteInt32(ClientPacket.CAdminWarp)
            buffer.WriteInt32(x)
            buffer.WriteInt32(y)

            Socket.SendData(buffer.ToPacket)
            buffer.Dispose()
        End Sub

        Friend Sub SendLeaveGame()
            Dim buffer As New ByteStream(4)

            buffer.WriteInt32(ClientPacket.CQuit)

            Socket.SendData(buffer.ToPacket)
            buffer.Dispose()
        End Sub

        Sub SendUnequip(eqNum As Integer)
            Dim buffer As New ByteStream(4)

            buffer.WriteInt32(ClientPacket.CUnequip)
            buffer.WriteInt32(eqNum)

            Socket.SendData(buffer.ToPacket)
            buffer.Dispose()
        End Sub

        Friend Sub ForgetSkill(skillslot As Integer)
            Dim buffer As New ByteStream(4)

            ' Check for subscript out of range
            If skillslot < 1 OrElse skillslot > MAX_PLAYER_SKILLS Then Exit Sub

            ' dont let them forget a skill which is in CD
            If SkillCd(skillslot) > 0 Then
                AddText("Cannot forget a skill which is cooling down!", QColorType.AlertColor)
                Exit Sub
            End If

            ' dont let them forget a skill which is buffered
            If SkillBuffer = skillslot Then
                AddText("Cannot forget a skill which you are casting!", QColorType.AlertColor)
                Exit Sub
            End If

            If PlayerSkills(skillslot) > 0 Then
                buffer.WriteInt32(ClientPacket.CForgetSkill)
                buffer.WriteInt32(skillslot)
                Socket.SendData(buffer.ToPacket)
            Else
                AddText("No skill found.", QColorType.AlertColor)
            End If

            buffer.Dispose()
        End Sub

        Friend Sub SendRequestMapreport()
            Dim buffer As New ByteStream(4)

            buffer.WriteInt32(ClientPacket.CMapReport)

            Socket.SendData(buffer.ToPacket)
            buffer.Dispose()
        End Sub

        Friend Sub SendRequestAdmin()
            Dim buffer As New ByteStream(4)

            buffer.WriteInt32(ClientPacket.CAdmin)

            Socket.SendData(buffer.ToPacket)
            buffer.Dispose()
        End Sub

        Friend Sub SendRequestClasses()
            Dim buffer As New ByteStream(4)

            buffer.WriteInt32(ClientPacket.CRequestClasses)

            Socket.SendData(buffer.ToPacket)
            buffer.Dispose()
        End Sub

        Friend Sub SendUseEmote(emote As Integer)
            Dim buffer As New ByteStream(4)

            buffer.WriteInt32(ClientPacket.CEmote)
            buffer.WriteInt32(emote)

            Socket.SendData(buffer.ToPacket)
            buffer.Dispose()
        End Sub
    End Module
End Namespace










































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































































