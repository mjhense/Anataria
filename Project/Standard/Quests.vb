Imports System.Xml

Public Class Quests

    Private p_current As Integer
    Private p_quest As Quest
    Private p_dialog As Dialogue
    Private p_game As Game
    Private IsTrue As Boolean = False
    Private p_enable As Boolean
    Public quests As List(Of Quest)
    Public killCount As Integer = 0

    Public Sub New(ByRef game As Game)
        quests = New List(Of Quest)
        p_current = -1
        p_quest = New Quest()
        p_game = game
        p_dialog = New Dialogue(p_game)
        p_enable = False
    End Sub

    Private Function getElement(ByVal field As String, ByRef element As XmlElement) As String
        Dim value As String = ""
        Try
            value = element.GetElementsByTagName(field)(0).InnerText
        Catch ex As Exception
            REM ignore error, just return empty
            Console.WriteLine(ex.Message)
        End Try
        Return value
    End Function

    Public Function Load(ByVal filename As String) As Boolean
        Try
            REM clear existing elements
            quests.Clear()

            REM open the xml file 
            Dim doc As New XmlDocument()
            doc.Load(filename)
            Dim list As XmlNodeList = doc.GetElementsByTagName("quest")
            For Each node As XmlNode In list

                REM get next record in table
                Dim element As XmlElement = node
                Dim q As New Quest()

                REM store fields
                q.Title = getElement("title", element)
                q.Summary = getElement("summary", element)
                q.Description = getElement("description", element)
                q.RequiredItem = getElement("req_item", element)
                q.RequiredItemCount = getElement("req_item_count", element)
                q.RequiredItemFlag = getElement("req_item_flag", element)
                q.RequiredLocFlag = getElement("req_loc_flag", element)
                q.RequiredLocX = getElement("req_loc_x", element)
                q.RequiredLocY = getElement("req_loc_y", element)
                q.RequiredNPCFlag = getElement("req_npc_flag", element)
                q.RequiredNPC = getElement("req_npc", element)
                q.RequiredKillNPCFlag = getElement("req_kill_flag", element)
                q.RequiredKillNPC = getElement("req_kill_npc", element)
                q.RequiredKillCount = getElement("req_kill_count", element)
                q.RewardGold = getElement("reward_gold", element)
                q.RewardItem = getElement("reward_item", element)
                q.RewardXP = getElement("reward_xp", element)
                q.RequiredPortalFile = getElement("req_portal_file", element)

                REM add new item to list
                quests.Add(q)
            Next
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function

    Public Property QuestNumber() As Integer
        Get
            Return p_current
        End Get
        Set(ByVal value As Integer)
            REM update position
            p_current = value
            If p_current < 0 Then
                p_current = 0
            ElseIf p_current > quests.Count - 1 Then
                p_current = quests.Count - 1
            End If

            REM update active quest
            p_quest = GetQuest(p_current)

        End Set
    End Property

    Public ReadOnly Property QuestItem() As Quest
        Get
            Return p_quest
        End Get
    End Property

    Public Property Enabled() As Boolean
        Get
            Return p_enable
        End Get
        Set(ByVal value As Boolean)
            p_enable = value
        End Set
    End Property

    Public Function QuestComplete() As Boolean
        Dim itemcount As Integer
        Dim itemname As String
        Dim sitex, sitey As Integer
        Dim absX, absY As Integer
        Dim tileX, tileY As Integer
        REM look for required quest items
        If p_quest.RequiredItemFlag Then
            itemcount = p_quest.RequiredItemCount
            itemname = p_quest.RequiredItem
            REM check inventory for item
            Dim count = p_game.inven.ItemCount(itemname)
            If count < itemcount Then Return False
        End If

        REM look for required location
        If p_quest.RequiredLocFlag Then
            sitex = p_quest.RequiredLocX
            sitey = p_quest.RequiredLocY
            absX = p_game.world.ScrollPos.X + p_game.hero.X + 48
            absY = p_game.world.ScrollPos.Y + p_game.hero.Y + 48 + 24
            tileX = absX \ 32
            tileY = absY \ 32
            If sitex <> -1 And sitey <> -1 Then
                If tileX <> sitex Or tileY <> sitey Then
                    Return False
                End If
            End If
            If p_quest.RequiredPortalFile <> p_game.world.CurrentLevel Then Return False
        End If

            REM look for both
            If p_quest.RequiredKillNPCFlag And p_quest.RequiredNPCFlag Then
                If NPCs() And NPCKills() Then
                    Return True
                Else
                    Return False
                End If
            End If

            REM look for required NPC
            If p_quest.RequiredNPCFlag Then
                If NPCs() Then
                    Return True
                Else
                    Return False
                End If
            End If

            REM look for required NPC kills
            If p_quest.RequiredKillNPCFlag Then
                If NPCKills() Then
                    Return True
                Else
                    Return False
                End If
            End If

            Return True
    End Function

    Public Function GetQuest(ByVal index As Integer) As Quest
        If index >= 0 And index < quests.Count - 1 Then
            Return quests.Item(index)
        End If
        Return Nothing
    End Function

    Public Function GetQuest(ByVal title As String) As Quest
        If title.Length > 0 Then
            For Each q As Quest In quests
                If q.Title = title Then Return q
            Next
        End If
        Return Nothing
    End Function

    Public Property Visible() As Boolean
        Get
            Return p_dialog.Visible
        End Get
        Set(ByVal value As Boolean)
            p_dialog.Visible = value
        End Set
    End Property

    Public Property Selection() As Integer
        Get
            Return p_dialog.Selection
        End Get
        Set(ByVal value As Integer)
            p_dialog.Selection = value
        End Set
    End Property

    Public Sub updateMouse(ByVal pos As Point, ByVal btn As MouseButtons)
        p_dialog.updateMouse(pos, btn)
    End Sub

    Public Sub Draw()
        If Not p_enable Then
            p_dialog.Title = "No Current Quest"
            p_dialog.Message = "You do not have a quest at this time."
            p_dialog.NumButtons = 1
            p_dialog.setButtonText(1, "Close")
        Else
            If Not QuestComplete() Then
                p_dialog.Title = p_quest.Title
                If QuestComplete() Then
                    p_dialog.Title += " (COMPLETE)"
                End If
                p_dialog.Message = p_quest.Description
                p_dialog.NumButtons = 0
            else
                p_dialog.Title = p_quest.Title
                If QuestComplete() Then
                    p_dialog.Title += " (COMPLETE)"
                End If
                p_dialog.Message = p_quest.Description
                p_dialog.NumButtons = 1
                p_dialog.setButtonText(1, "Finish")
            End If
        End If

        p_dialog.Draw()
    End Sub

    Public Function NPCKills() As Boolean
        Dim npctarget As New Character(p_game)
        Dim name As String = p_quest.RequiredKillNPC
        Dim index As Integer = 0
        Select Case p_quest.RequiredKillNPC
            Case "skeleton"
                npctarget.Name = "skeleton"
            Case "Viking Gaurd"
                npctarget.Name = "Viking Gaurd"
            Case "Dark Knight"
                npctarget.Name = "Dark Knight"
            Case "Pyro Raptor"
                npctarget.Name = "Pyro Raptor"
            Case "Lava Troll"
                npctarget.Name = "Lava Troll"
            Case "Spider"
                npctarget.Name = "Red Spider"
            Case "Dark Knight"
                npctarget.Name = "Dark Knight"
            Case "skeleton archer", "skeleton Archer", "Skeleton Archer"
                npctarget.Name = "Skeleton Archer"
            Case "Grass Spider"
                npctarget.Name = "Grass Spider"
            Case "Zombie", "zombie"
                npctarget.Name = "Zombie"
            Case Else
                npctarget.Name = p_quest.RequiredKillNPC
        End Select
        If killCount >= p_quest.RequiredKillCount Then
            Return True
            killCount = 0
        Else
            Return False
        End If
    End Function

    Public Function NPCs() As Boolean
        Dim npc As New Character(p_game)
        Select Case p_quest.RequiredNPC
            Case "TutorialKnight1"
                npc = p_game.tutorialKnight1
            Case "TutorialKnight2"
                npc = p_game.tutorialKnight2
            Case "BlueKnight"
                npc = p_game.BlueKnight
            Case "Good King"
                npc = p_game.GoodKing
            Case "Viking King"
                npc = p_game.VikingKing
            Case "PeasantAnna"
                npc = p_game.PeasantAnna
            Case "Vendor"
                npc = p_game.Vendor
            Case "Wounded Farmer"
                npc = p_game.Peasant
            Case "Urix"
                npc = p_game.Urix
        End Select
        Dim talkFlag As Boolean = False
        Dim relativePos As PointF
        Const talkRadius As Integer = 70
        Dim heroCenter As PointF
        Dim npcCenter As PointF
        Dim dist As Single
        Dim index As Integer = 0

        If npc IsNot Nothing Then
            If npc.X > p_game.world.ScrollPos.X - 64 _
            And npc.X < p_game.world.ScrollPos.X + 23 * 32 + 64 _
            And npc.Y > p_game.world.ScrollPos.Y - 54 _
            And npc.Y < p_game.world.ScrollPos.Y + 17 * 32 Then

                heroCenter = p_game.hero.CenterPos
                relativePos.X = npc.X - p_game.world.ScrollPos.X
                relativePos.Y = npc.Y - p_game.world.ScrollPos.Y
                npc.Draw(relativePos)

                REM get center of NPC
                npcCenter = relativePos
                npcCenter.X += npc.GetSprite.Width / 2
                npcCenter.Y += npc.GetSprite.Height / 2

                REM get distance to the NPC
                dist = p_game.hero.CenterDistance(npcCenter)

                REM is player trying to talk to this NPC?
                If dist < talkRadius And npc.Alive Then
                    p_game.Device.DrawEllipse(New Pen(Color.Blue, 2.0), npcCenter.X - 24, npcCenter.Y, 48, 48)
                    talkFlag = True
                Else
                    talkFlag = False
                End If

                If talkFlag = True Then
                    Return True
                Else
                    Return False
                End If
            End If
        End If
    End Function

End Class
