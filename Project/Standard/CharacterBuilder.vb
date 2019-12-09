Public Class CharacterBuilder
    Private p_game As Game
    Private p_bg As Bitmap
    Private p_arrow As Bitmap
    Private p_font24 As Font
    Private p_font36 As Font
    Private p_oldButton As MouseButtons
    Private p_warrior As Character
    Private p_paladin As Character
    Private p_hunter As Character
    Private p_priest As Character
    Private crocodile As Character
    Private ninja As Character
    Private skeletonArcher As Character
    Private skeleton As Character
    Private skeletonWarrior As Character

    Private Structure modifiers
        Dim STR As Integer
        Dim DEX As Integer
        Dim STA As Integer
        Dim INT As Integer
        Dim CHA As Integer
        Dim HP As Integer
        Dim ATK As Integer
        Dim DEF As Integer
    End Structure
    Private p_mods As modifiers
    Private p_stats As modifiers

    Private p_class As Integer
    Private classes() As String = { _
        "Warrior", _
        "Paladin", _
        "Hunter", _
        "Priest"
    }

    Private p_race As Integer
    Private races() As String = { _
        "Human", _
        "Undead", _
        "Dragonkin" _
    }

    Private p_element As Integer
    Private elements() As String = {
        "Fire",
        "Holy",
        "Dark",
        "Thunder"
    }

    Private btnName As Sprite
    Private btnClass As Sprite
    Private btnRace As Sprite
    Private btnRoll As Sprite
    Private btnSave As Sprite
    Private btnElement As Sprite

    Public Sub New(ByRef game As Game)
        p_game = game

        Try
            p_font24 = New Font("Orbitron", 24, FontStyle.Regular, GraphicsUnit.Pixel)
            p_font36 = New Font("Orbitron", 36, FontStyle.Regular, GraphicsUnit.Pixel)
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try

        REM load background
        p_bg = p_game.LoadBitmap("character bg.png")
        If p_bg Is Nothing Then
            MessageBox.Show("Error loading character bg.png")
            End
        End If

        REM create arrow buttons
        p_arrow = p_game.LoadBitmap("arrow.png")
        If p_arrow Is Nothing Then
            MessageBox.Show("Error loading arrow.png")
            End
        End If

        REM name button
        btnName = New Sprite(p_game)
        btnName.Image = p_arrow
        btnName.Position = New Point(208, 154)
        btnName.Size = New Size(20, 20)

        REM class button
        btnClass = New Sprite(p_game)
        btnClass.Image = p_arrow
        btnClass.Position = New Point(208, 184)
        btnClass.Size = New Size(20, 20)

        REM race button
        btnRace = New Sprite(p_game)
        btnRace.Image = p_arrow
        btnRace.Position = New Point(208, 212)
        btnRace.Size = New Size(20, 20)

        REM element button
        btnElement = New Sprite(p_game)
        btnElement.Image = p_arrow
        btnElement.Position = New Point(208 + 280 + 35, 212)
        btnElement.Size = New Size(20, 20)

        REM roll button
        btnRoll = New Sprite(p_game)
        btnRoll.Image = p_game.LoadBitmap("button roll.png")
        btnRoll.Position = New Point(90, 510)
        btnRoll.Size = New Size(60, 30)

        REM save button
        btnSave = New Sprite(p_game)
        btnSave.Image = p_game.LoadBitmap("button save.png")
        btnSave.Position = New Point(560, 510)
        btnSave.Size = New Size(60, 30)

        REM load character sprites
        p_warrior = New Character(p_game)
        If Not p_warrior.Load("hero sword.char") Then
            MessageBox.Show("Error loading hero sword.char")
            End
        End If
        p_warrior.Position = New PointF(638, 133)
        p_warrior.Direction = 6
        p_warrior.AnimationState = Character.AnimationStates.Walking

        p_paladin = New Character(p_game)
        If Not p_paladin.Load("hero axe shield.char") Then
            MessageBox.Show("Error loading hero axe shield.char")
            End
        End If
        p_paladin.Position = p_warrior.Position
        p_paladin.Direction = p_warrior.Direction
        p_paladin.AnimationState = p_warrior.AnimationState

        p_hunter = New Character(p_game)
        If Not p_hunter.Load("hero bow.char") Then
            MessageBox.Show("Error loading hero bow.char")
            End
        End If
        p_hunter.Position = p_warrior.Position
        p_hunter.Direction = p_warrior.Direction
        p_hunter.AnimationState = p_warrior.AnimationState

        p_priest = New Character(p_game)
        If Not p_priest.Load("hero staff.char") Then
            MessageBox.Show("Error loading hero staff.char")
            End
        End If
        p_priest.Position = p_warrior.Position
        p_priest.Direction = p_warrior.Direction
        p_priest.AnimationState = p_warrior.AnimationState
        crocodile = New Character(p_game)
        If Not crocodile.Load("Croc Warrior.char") Then
            MessageBox.Show("Error loading Croc Warrior.char")
            End
        End If
        crocodile.Position = p_warrior.Position
        crocodile.Direction = p_warrior.Direction
        crocodile.AnimationState = p_warrior.AnimationState
        skeleton = New Character(p_game)
        If Not skeleton.Load("Reaper.char") Then
            MessageBox.Show("Error loading skeleton unarmed.char")
            End
        End If
        skeleton.Position = New PointF(622, 117)
        skeleton.Direction = p_warrior.Direction
        skeleton.AnimationState = p_warrior.AnimationState
        skeletonArcher = New Character(p_game)
        If Not skeletonArcher.Load("hero bow skel.char") Then
            MessageBox.Show("Error loading skeleton archer.char")
            End
        End If
        skeletonArcher.Position = p_warrior.Position
        skeletonArcher.Direction = p_warrior.Direction
        skeletonArcher.AnimationState = p_warrior.AnimationState
        skeletonWarrior = New Character(p_game)
        If Not skeletonWarrior.Load("hero sword skel.char") Then
            MessageBox.Show("Error loading skeleton sword shield.char")
            End
        End If
        skeletonWarrior.Position = p_warrior.Position
        skeletonWarrior.Direction = p_warrior.Direction
        skeletonWarrior.AnimationState = p_warrior.AnimationState

        p_element = 0
        p_game.hero.Element = elements(p_element)
        p_class = 0
        SetClassModifiers(classes(p_class))
    End Sub

    Private Sub Print(ByVal x As Integer, ByVal y As Integer, ByVal text As String, ByVal color As Brush)
        p_game.Device.DrawString(text, p_font24, color, x, y)
    End Sub

    Private Sub PrintRight(ByVal x As Integer, ByVal y As Integer, ByVal text As String, ByVal color As Brush)
        Dim rsize As SizeF
        rsize = p_game.Device.MeasureString(text, p_font24)
        p_game.Device.DrawString(text, p_font24, color, x - rsize.Width, y)
    End Sub

    Private Function DieRoll(ByVal dots As Integer) As Integer
        Return p_game.Random(1, dots)
    End Function

    Private Sub RollStats()
        p_stats.STR = DieRoll(6) + DieRoll(6)
        p_stats.DEX = DieRoll(6) + DieRoll(6)
        p_stats.STA = DieRoll(6) + DieRoll(6)
        p_stats.INT = DieRoll(6) + DieRoll(6)
        p_stats.CHA = DieRoll(6) + DieRoll(6)
    End Sub

    Private Sub ModStats()
        p_game.hero.STR = p_stats.STR + p_mods.STR
        p_game.hero.DEX = p_stats.DEX + p_mods.DEX
        p_game.hero.STA = p_stats.STA + p_mods.STA
        p_game.hero.INT = p_stats.INT + p_mods.INT
        p_game.hero.CHA = p_stats.CHA + p_mods.CHA

        p_game.hero.HitPoints = p_game.hero.STA + DieRoll(8)
    End Sub

    Private Sub SetClassModifiers(ByVal pclass As String)
        Select Case pclass
            Case "Warrior"
                p_mods.STR = 8
                p_mods.DEX = 3
                p_mods.STA = 4
                p_mods.INT = 0
                p_mods.CHA = 0
            Case "Paladin"
                p_mods.STR = 3
                p_mods.DEX = 3
                p_mods.STA = 8
                p_mods.INT = 0
                p_mods.CHA = 1
            Case "Hunter"
                p_mods.STR = 2
                p_mods.DEX = 8
                p_mods.STA = 4
                p_mods.INT = 0
                p_mods.CHA = 1
            Case "Priest"
                p_mods.STR = 0
                p_mods.DEX = 6
                p_mods.STA = 1
                p_mods.INT = 8
                p_mods.CHA = 0
        End Select
    End Sub

    Public Sub Draw()
        Static c1 As Brush = Brushes.White
        Static c2 As Brush = Brushes.Yellow
        Static c3 As Brush = Brushes.LightBlue
        Static c4 As Brush = Brushes.LightGreen

        p_game.Device.DrawImage(p_bg, 0, 0, 800, 600)
        p_game.Device.DrawString("CREATE CHARACTER", p_font36, Brushes.White, 200, 75)

        Select Case p_game.hero.PlayerClass
            Case "Warrior"
                Select Case p_game.hero.Race
                    Case "Human"
                        p_warrior.Draw()
                    Case "Undead"
                        skeleton.Draw()
                    Case "Dragonkin"
                        crocodile.Draw()
                    Case Else
                        p_warrior.Draw()
                End Select
            Case "Paladin"
                If p_game.hero.Race = "Human" Then
                    p_paladin.Draw()
                ElseIf p_game.hero.Race = "Undead" Then
                    skeletonWarrior.Draw()
                ElseIf p_game.hero.Race = "Dragonkin" Then
                    crocodile.Draw()
                Else
                    p_paladin.Draw()
                End If
            Case "Hunter"
                Select Case p_game.hero.Race
                    Case "Human"
                        p_hunter.Draw()
                    Case "Undead"
                        skeletonArcher.Draw()
                    Case "Dragonkin"
                        crocodile.Draw()
                    Case Else
                        p_hunter.Draw()
                End Select
            Case "Priest"
                p_priest.Draw()
        End Select

        Dim x As Integer = 100
        Dim y As Integer = 150
        Print(x, y, "NAME", c1) : y += 30
        Print(x, y, "CLASS", c1) : y += 30
        Print(x, y, "RACE", c1)
        Print(x + 280, y, "ELEMENT", c1)
        y = 270
        Print(x, y, "STR", c1) : y += 30
        Print(x, y, "DEX", c1) : y += 30
        Print(x, y, "STA", c1) : y += 30
        Print(x, y, "INT", c1) : y += 30
        Print(x, y, "CHA", c1) : y += 30

        x = 235 : y = 150
        Print(x, y, p_game.hero.Name, c2) : y += 30
        Print(x, y, p_game.hero.PlayerClass, c2) : y += 30
        Print(x, y, p_game.hero.Race, c2)
        Print(x + 280 + 35, y, p_game.hero.Element, c2)

        x = 230 : y = 270
        PrintRight(x, y, p_stats.STR, c2) : y += 30
        PrintRight(x, y, p_stats.DEX, c2) : y += 30
        PrintRight(x, y, p_stats.STA, c2) : y += 30
        PrintRight(x, y, p_stats.INT, c2) : y += 30
        PrintRight(x, y, p_stats.CHA, c2)

        x = 300 : y = 270
        PrintRight(x, y, "+  " + p_mods.STR.ToString(), c3) : y += 30
        PrintRight(x, y, "+  " + p_mods.DEX.ToString(), c3) : y += 30
        PrintRight(x, y, "+  " + p_mods.STA.ToString(), c3) : y += 30
        PrintRight(x, y, "+  " + p_mods.INT.ToString(), c3) : y += 30
        PrintRight(x, y, "+  " + p_mods.CHA.ToString(), c3)

        x = 380 : y = 270
        PrintRight(x, y, "=  " + p_game.hero.STR.ToString(), c4) : y += 30
        PrintRight(x, y, "=  " + p_game.hero.DEX.ToString(), c4) : y += 30
        PrintRight(x, y, "=  " + p_game.hero.STA.ToString(), c4) : y += 30
        PrintRight(x, y, "=  " + p_game.hero.INT.ToString(), c4) : y += 30
        PrintRight(x, y, "=  " + p_game.hero.CHA.ToString(), c4)

        REM calculated data
        x = 480 : y = 270
        Print(x, y, "H.P.", c1) : x += 80
        Print(x, y, p_game.hero.HitPoints, c3) : y += 30 : x -= 80


        REM draw buttons
        btnClass.Draw()
        btnName.Draw()
        btnRace.Draw()
        btnRoll.Draw()
        btnSave.Draw()
        btnElement.Draw()

        REM look for button release 
        If p_game.MouseButton = MouseButtons.None And p_oldButton <> MouseButtons.None Then
            Dim mouseRect As New Rectangle(p_game.MousePos.X, p_game.MousePos.Y, 1, 1)

            REM name button
            If btnName.Bounds().IntersectsWith(mouseRect) Then
                p_oldButton = MouseButtons.None
                p_game.hero.Name = InputBox("Enter your character's name:", "NAME", p_game.hero.Name)
                If p_game.hero.Name.Length > 20 Then
                    p_game.hero.Name = p_game.hero.Name.Substring(0, 19)
                End If
            End If

            REM class button
            If btnClass.Bounds().IntersectsWith(mouseRect) Then
                p_oldButton = MouseButtons.None
                p_class += 1
                If p_class > classes.Length - 1 Then p_class = 0
                p_game.hero.PlayerClass = classes(p_class)
                SetClassModifiers(classes(p_class))
                ModStats()
            End If

            REM race button
            If btnRace.Bounds().IntersectsWith(mouseRect) Then
                p_oldButton = MouseButtons.None
                p_race += 1
                If p_race > races.Length - 1 Then p_race = 0
                p_game.hero.Race = races(p_race)
            End If

            REM roll button
            If btnRoll.Bounds().IntersectsWith(mouseRect) Then
                p_oldButton = MouseButtons.None
                RollStats()
                ModStats()
            End If

            REM save button
            If btnSave.Bounds().IntersectsWith(mouseRect) Then
                p_oldButton = MouseButtons.None
                SaveNewPlayer()
                p_game.gameState = Game.GameStates.STATE_TITLE
            End If

            REM element button
            If btnElement.Bounds.IntersectsWith(mouseRect) Then
                p_oldButton = MouseButtons.None
                p_element += 1
                If p_element > elements.Length - 1 Then
                    p_element = 0
                End If
                p_game.hero.Element = elements(p_element)
            End If
        End If
        p_oldButton = p_game.MouseButton
    End Sub

    Private Sub SaveNewPlayer()
        p_game.hero.PlayerClass = classes(p_class)
        p_game.hero.Level = 1
        p_game.hero.Experience = 0
        p_game.hero.Description = "Generated human player"
        p_game.hero.DropGoldMin = 0
        p_game.hero.DropGoldMax = 0
        p_game.hero.DropItem1 = ""
        p_game.hero.DropItem2 = ""
        p_game.hero.DropItem3 = ""
        p_game.hero.DropNum1 = 0
        p_game.hero.DropNum2 = 0
        p_game.hero.DropNum3 = 0
        p_game.hero.Gold = 0
        p_game.hero.Race = races(p_race)
        p_game.world.ScrollPos = New Point(50, 50)
        p_game.hero.Position = New PointF(400 - 48, 300 - 48)
        p_game.world.LastSpawn = p_game.hero.GetCurrentTilePos
        p_game.quests.QuestNumber = 0
        p_game.world.CurrentLevel = "Training Tutorial.level"
        p_game.inven.ClearInventory()
        p_game.hero.SaveGame(Game.savegamefile)
        p_game.hero.Element = elements(p_element)
        PlaySave()
    End Sub

End Class