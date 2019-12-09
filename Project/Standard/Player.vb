Imports System.Xml
Public Class Player
    Inherits Character
    Private p_game As Game
    Private p_gold As Integer
    Public Sub New(ByRef game As Game)
        MyBase.New(game)
        p_game = game
        '    fireSprite = p_game.ReturnFireSprite(p_game, New Point(Me.Position.X + 32, Me.Position.Y + 32))
    End Sub
    Public ReadOnly Property WorldPosition As PointF
        Get
            Return New PointF(p_game.world.X + Me.X, p_game.world.Y + Me.Y)
        End Get
    End Property
    Public Property Gold() As Integer
        Get
            Return p_gold
        End Get
        Set(ByVal value As Integer)
            p_gold = value
        End Set
    End Property
    Public Function AddCombatXP(ByRef npc As Character) As Integer
        Dim xp As Integer
        If npc.Level = 0 Then npc.Level = 1
        xp = npc.Level * p_game.Random(10, 20)
        Experience += xp
        REM level up?
        If Experience > 1000 * (0.2 * Level) Then
            Level += 1
            p_game.hero.Health = p_game.hero.HitPoints
            Experience = 0
            LevelStats()
            PlayLevelUp()
            Dim fnt As New Font("Narkisim", 24)
            Dim sz As SizeF = p_game.Device.MeasureString("Level Up!", fnt)
            p_game.AddFadingText(p_game.hero.GetSprite.X + (p_game.hero.GetSprite().Width / 2) - (sz.Width / 2), p_game.hero.GetSprite.Y - 20, "Level Up!", 1.0F, Color.Yellow, fnt, True, New PointF(0, -2.0F))
        End If
        Return xp
    End Function
    Public Function AddCombatXP(xp As Integer) As Integer
        Experience += xp
        REM level up?
        If Experience > 1000 * (0.2 * Level) Then
            Level += 1
            p_game.hero.Health = p_game.hero.HitPoints
            Experience = 0
            LevelStats()
            PlayLevelUp()
            Dim fnt As New Font("Narkisim", 24)
            Dim sz As SizeF = p_game.Device.MeasureString("Level Up!", fnt)
            p_game.AddFadingText(p_game.hero.GetSprite.X + (p_game.hero.GetSprite().Width / 2) - (sz.Width / 2), p_game.hero.GetSprite.Y - 20, "Level Up!", 1.0F, Color.Yellow, fnt, True, New PointF(0, -2.0F))
        End If
        Return xp
    End Function
    Public Overrides Function GetCurrentTilePos() As Point
        Try
            Dim feet As PointF = p_game.hero.FootPos()
            Dim tilex As Integer = (p_game.world.ScrollPos.X + feet.X) / 32
            Dim tiley As Integer = (p_game.world.ScrollPos.Y + feet.Y) / 32
            Return New Point(tilex, tiley)
        Catch
        End Try
    End Function
    Public Overrides Function GetSpriteCurrentTilePos() As Point
        Try
            Return New Point(Convert.ToInt32((X + p_game.world.X) / 32), Convert.ToInt32((Y + p_game.world.Y) / 32))
        Catch
        End Try
    End Function
    Public Function GetCurrentTileRect() As Rectangle
        Try
            Dim feet As PointF = p_game.hero.FootPos()
            Dim tilex As Integer = (p_game.world.ScrollPos.X + feet.X - 24)
            Dim tiley As Integer = (p_game.world.ScrollPos.Y + feet.Y - 24)
            Return New Rectangle(tilex, tiley, tilex + 24, tiley + 24)
        Catch
        End Try
    End Function
    Public Function GetWeaponDamage() As Integer
        Dim die, num, atk As Integer
        Dim weapon As Item = p_game.inven.GetItem(p_game.inven.BTN_RTHAND)
        If weapon IsNot Nothing Then
            die = weapon.AttackDie 'e.g. 2D6
            num = weapon.AttackNumDice
            For n = 1 To num
                atk += p_game.Random(die)
            Next
        End If
        weapon = p_game.inven.GetItem(p_game.inven.BTN_LTHAND)
        If weapon IsNot Nothing Then
            die = weapon.AttackDie 'e.g. 2D6
            num = weapon.AttackNumDice
            For n = 1 To num
                atk += p_game.Random(die)
            Next
        End If
        Return atk
    End Function
    Public Function GetArmorValue() As Integer
        Dim armor As Integer = 0

        REM add chest armor to total armor value
        Dim chest As Item = p_game.inven.GetItem(p_game.inven.BTN_CHEST)
        If chest IsNot Nothing Then
            armor += chest.Defense
        End If

        REM add helm armor
        Dim helm As Item = p_game.inven.GetItem(p_game.inven.BTN_HEAD)
        If helm IsNot Nothing Then
            armor += helm.Defense
        End If

        REM add boot armor
        Dim legs As Item = p_game.inven.GetItem(p_game.inven.BTN_LEGS)
        If legs IsNot Nothing Then
            armor += legs.Defense
        End If

        REM check rt hand
        Dim rthand As Item = p_game.inven.GetItem(p_game.inven.BTN_RTHAND)
        If rthand IsNot Nothing Then
            armor += rthand.Defense
        End If

        REM check lt hand
        Dim lthand As Item = p_game.inven.GetItem(p_game.inven.BTN_LTHAND)
        If lthand IsNot Nothing Then
            armor += lthand.Defense
        End If

        REM check rt finger
        Dim rtfinger As Item = p_game.inven.GetItem(p_game.inven.BTN_RTFINGER)
        If rtfinger IsNot Nothing Then
            armor += rtfinger.Defense
        End If

        REM check lt finger
        Dim ltfinger As Item = p_game.inven.GetItem(p_game.inven.BTN_LTFINGER)
        If ltfinger IsNot Nothing Then
            armor += ltfinger.Defense
        End If

        Return armor
    End Function
    Public Overrides Function ToString() As String
        Return MyBase.Name
    End Function
    Private Function GetElement(ByVal field As String, ByRef element As XmlElement) As String
        Dim value As String = ""
        Try
            value = element.GetElementsByTagName(field)(0).InnerText
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try
        Return value
    End Function
    Public Sub LoadGame(ByVal filename As String)
        Try
            REM open the xml file 
            Dim doc As New XmlDocument()
            doc.Load(filename)
            Dim list As XmlNodeList = doc.GetElementsByTagName("gamestate")
            Dim element As XmlElement = list(0)

            REM read data fields
            p_game.hero.PlayerClass = GetElement("class", element)
            p_game.hero.Race = GetElement("race", element)

            REM load proper character
            REM load default animations based on class and race
            Select Case p_game.hero.PlayerClass
                Case "Warrior"
                    Select Case Race
                        Case "Human"
                            p_game.hero.Load("hero sword.char")
                        Case "Undead"
                            p_game.hero.Load("skeleton unarmed.char")
                        Case "Dragonkin"
                            p_game.hero.Load("Croc Warrior.char")
                    End Select
                Case "Paladin"
                    Select Case Race
                        Case "Human"
                            p_game.hero.Load("hero axe shield.char")
                        Case "Undead"
                            p_game.hero.Load("skeleton sword shield.char")
                        Case "Dragonkin"
                            p_game.hero.Load("Croc Warrior.char")
                    End Select
                Case "Hunter"
                    Select Case Race
                        Case "Human"
                            p_game.hero.Load("hero bow.char")
                        Case "Undead"
                            p_game.hero.Load("skeleton archer.char")
                        Case "Dragonkin"
                            p_game.hero.Load("Croc Warrior.char")
                    End Select
                Case "Priest"
                    p_game.hero.Load("hero staff.char")
            End Select

            p_game.hero.Name = GetElement("name", element)
            p_game.hero.Level = Convert.ToInt32(GetElement("level", element))
            p_game.hero.Experience = Convert.ToInt32(GetElement("xp", element))
            p_game.hero.HitPoints = Convert.ToInt32(GetElement("hp", element))
            p_game.hero.STR = Convert.ToInt32(GetElement("str", element))
            p_game.hero.DEX = Convert.ToInt32(GetElement("dex", element))
            p_game.hero.STA = Convert.ToInt32(GetElement("sta", element))
            p_game.hero.INT = Convert.ToInt32(GetElement("int", element))
            p_game.hero.CHA = Convert.ToInt32(GetElement("cha", element))
            p_game.quests.QuestNumber = Convert.ToInt32(GetElement("quest", element))
            p_game.world.X = Convert.ToInt32(GetElement("scrollx", element))
            p_game.world.Y = Convert.ToInt32(GetElement("scrolly", element))
            p_game.world.CurrentLevel = (GetElement("currentLevel", element))
            p_game.world.loadTilemap(p_game.world.CurrentLevel)
            p_game.hero.Gold = Convert.ToInt32(GetElement("gold", element))
            p_game.hero.X = CInt(GetElement("herox", element))
            p_game.hero.Y = CInt(GetElement("heroy", element))

            p_game.world.LastSpawn = p_game.hero.GetCurrentTilePos

            p_game.hero.Element = Convert.ToString(GetElement("element", element))

            Dim itm As Item
            For n = 0 To 9
                itm = p_game.items.GetItem(GetElement("item0" + n.ToString(), element))
                If itm IsNot Nothing Then
                    p_game.inven.SetItem(itm, n)
                End If
            Next
            For n = 10 To 30
                itm = p_game.items.GetItem(GetElement("item" + n.ToString(), element))
                If itm IsNot Nothing Then
                    p_game.inven.SetItem(itm, n)
                End If
            Next

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub
    Public Sub SaveGame(ByVal filename As String)
        Try
            REM create data type templates
            Dim typeInt As System.Type = System.Type.GetType("System.Int32")
            Dim typeBool As System.Type = System.Type.GetType("System.Boolean")
            Dim typeStr As System.Type = System.Type.GetType("System.String")

            REM create xml schema
            Dim table As New DataTable("gamestate")
            table.Columns.Add(New DataColumn("name", typeStr))
            table.Columns.Add(New DataColumn("class", typeStr))
            table.Columns.Add(New DataColumn("race", typeStr))
            table.Columns.Add(New DataColumn("level", typeInt))
            table.Columns.Add(New DataColumn("xp", typeInt))
            table.Columns.Add(New DataColumn("hp", typeInt))
            table.Columns.Add(New DataColumn("str", typeInt))
            table.Columns.Add(New DataColumn("dex", typeInt))
            table.Columns.Add(New DataColumn("sta", typeInt))
            table.Columns.Add(New DataColumn("int", typeInt))
            table.Columns.Add(New DataColumn("cha", typeInt))
            table.Columns.Add(New DataColumn("quest", typeInt))
            table.Columns.Add(New DataColumn("scrollx", typeInt))
            table.Columns.Add(New DataColumn("scrolly", typeInt))
            table.Columns.Add(New DataColumn("currentLevel", typeStr))
            table.Columns.Add(New DataColumn("gold", typeInt))
            table.Columns.Add(New DataColumn("element", typeStr))
            table.Columns.Add(New DataColumn("herox", typeInt))
            table.Columns.Add(New DataColumn("heroy", typeInt))
            table.Columns.Add(New DataColumn("mp", typeInt))

            table.Columns.Add(New DataColumn("item00", typeStr))
            For n = 1 To 9
                table.Columns.Add(New DataColumn("item0" + n.ToString(), typeStr))
            Next
            For n = 10 To 30
                table.Columns.Add(New DataColumn("item" + n.ToString(), typeStr))
            Next

            REM copy data into datatable
            Dim row As DataRow = table.NewRow()
            row("name") = Name
            row("class") = PlayerClass
            row("race") = Race
            row("level") = Level
            row("xp") = Experience
            row("hp") = HitPoints
            row("str") = STR
            row("dex") = DEX
            row("sta") = STA
            row("int") = INT
            row("cha") = CHA
            row("quest") = p_game.quests.QuestNumber
            row("scrollx") = p_game.world.ScrollPos.X
            row("scrolly") = p_game.world.ScrollPos.Y
            row("currentLevel") = p_game.world.CurrentLevel
            row("gold") = p_game.hero.Gold
            row("element") = p_game.hero.Element
            row("herox") = p_game.hero.X
            row("heroy") = p_game.hero.Y

            Dim itm As Item
            For n = 0 To 9
                itm = p_game.inven.GetItem(n)
                row("item0" + n.ToString()) = itm.Name
            Next
            For n = 10 To 30
                itm = p_game.inven.GetItem(n)
                row("item" + n.ToString()) = itm.Name
            Next
            table.Rows.Add(row)

            REM save xml file
            table.WriteXml(filename)
            table.Dispose()

        Catch es As Exception
            Stop
            MessageBox.Show(es.Message)
        End Try
    End Sub
    Public Overrides Sub UpdateStats()
        If Bleeding Then
            _bleedTime += (1000 / Form1.FrameRate)
            If Me.Alive Then
                Me.Health -= bleedPower
                p_game.combat.CheckPlayerDeath()
            End If
        End If
        If Frozen Then
            _freezeTime += (1000 / Form1.FrameRate)
            Me.iceSprite.Animate()
            Me.iceSprite.Draw(p_position.X + 24, p_position.Y)
        End If
        If Regenerating Then
            _regenTime += (1000 / Form1.FrameRate)
            If Me.drainVictim.Alive And (Me.Health < Me.HitPoints) Then
                Me.Health += CSng(regenPower * 0.05)
                Me.drainVictim.Health -= (regenPower * 0.5)
                p_game.combat.CheckEnemyDeath()
            Else
                If Me.Health > Me.HitPoints Then Me.Health = Me.HitPoints
            End If
        End If
        If _bleedTime >= 0 Then
            _bleedTime = 0.0F
            Bleeding = False
        End If
        If _regenTime >= 0 Then
            _regenTime = 0.0F
            Regenerating = False
        End If
        If _freezeTime >= 0 Then
            _freezeTime = 0.0F
            Frozen = False
        End If
    End Sub
    Public Sub LevelStats()
        Select Case PlayerClass
            Case "Warrior"
                STR += 8
                DEX += 3
                STA += 4
                INT += 0
                CHA += 0
            Case "Paladin"
                STR += 3
                DEX += 3
                STA += 8
                INT += 0
                CHA += 1
            Case "Hunter"
                STR += 2
                DEX += 8
                STA += 4
                INT += 0
                CHA += 1
            Case "Priest"
                STR += 0
                DEX += 6
                STA += 1
                INT += 8
                CHA += 0
        End Select
        HitPoints = p_game.hero.STA + (p_game.Random(1, 8) * Level)
        Health = HitPoints
    End Sub
    Public Overrides Sub UpdateTiles()
        oldTilePos = newTilePos
        For m As Integer = oldTilePos.Y To oldTilePos.Y + 1 Step 1
            Dim p As New PointF(GetCurrentTilePos.X, m)
            p_game.world.SetTempCollidableForMonsters(p, False)
        Next
        newTilePos = GetSpriteCurrentTilePos()
        For m As Integer = newTilePos.Y To newTilePos.Y + 1 Step 1
            Dim p As New PointF(GetCurrentTilePos.X, m)
            If New Rectangle(GetCurrentTilePos.X * 32, m * 32, 32, 32).IntersectsWith(New Rectangle(X + p_game.world.X, Y + p_game.world.Y, GetSprite.Width, GetSprite.Height)) Then
                If Me.Alive Then
                    p_game.world.SetTempCollidableForMonsters(p, True)
                Else
                    p_game.world.SetTempCollidableForMonsters(p, False)
                End If
            End If
        Next
    End Sub
End Class