Public Class Form1
    Private game As Game
    Private gameover As Boolean = False
    Private lootDialog As Dialogue
    Public lootFlag As Boolean = False
    Private lootTarget As Integer
    Private monstersInView As Integer
    Private monstersInRange As Integer
    Private font24 As New Font("Narkisim", 24, FontStyle.Regular, GraphicsUnit.Pixel)
    Private font36 As New Font("Narkisim", 36, FontStyle.Regular, GraphicsUnit.Pixel)
    Public talkFlag As Boolean = False
    Private dialog As Dialogue
    Public FrameRate As Integer = 0
    REM ****************************************************************************************************
    REM inventory glitch occurs because the copy command copies all items with the same name.
    REM Eventually you need stop purchase text from vender show multiple texts at the same time.
    REM ****************************************************************************************************
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        REM ***************************************
        REM create game object
        REM ***************************************
        ' game = New Game(Me, My.Computer.Screen.WorkingArea.Width, My.Computer.Screen.WorkingArea.Height)
        Game = New Game(Me, 840, 638)
        Me.FormBorderStyle = Windows.Forms.FormBorderStyle.Fixed3D

        REM state
        Game.gameState = Game.GameStates.STATE_TITLE

        REM ***************************************
        REM create title screen
        REM ***************************************
        Game.titleScreen = New Title(Game)

        REM ***************************************
        REM initialize the status manager
        REM ***************************************
        StatusIconManager.Initialize(Game)

        REM ***************************************
        REM create dialogue objects
        REM ***************************************
        Game.dialog = New Dialogue(Game)
        lootDialog = New Dialogue(Game)
        dialog = New Dialogue(Game)

        REM ***************************************
        REM create stats window
        REM ***************************************
        Game.stats = New Stats(Game)
        Game.stats.Visible = True

        REM ***************************************
        REM create monster list
        REM ***************************************
        Game.NPCs = New List(Of Character)

        REM ***************************************
        REM create treasure drop list
        REM ***************************************
        Game.treasure = New List(Of Game.DrawableItem)

        REM ***************************************
        REM create combat module
        REM ***************************************
        Game.combat = New Combat(Game)

        REM ***************************************
        REM load items
        REM note: items file can be loaded in script
        REM ***************************************
        Game.items = New Items()
        If Not Game.items.Load("items.item") Then
            MessageBox.Show("Error loading file default.item")
            End
        End If

        REM ***************************************
        REM load hero 
        REM ***************************************
        Game.hero = New Player(Game)
        Game.hero.Load("default.char")
        Game.hero.Position = New Point(400 - 48, 300 - 48)
        Game.hero.AnimationState = Character.AnimationStates.Standing

        REM ***************************************
        REM create inventory object
        REM ***************************************
        Game.inven = New Inventory(Game, New Point((800 - 532) / 2, 50))

        REM ***************************************
        REM create quest object
        REM ***************************************
        Game.quests = New Quests(Game)
        Game.quests.Load("quests.quest")
        Game.quests.QuestNumber = 0
        Game.quests.Enabled = True

        REM ***************************************
        REM character builder
        REM ***************************************
        Game.charBuilder = New CharacterBuilder(Game)



        REM ***************************************
        REM create tilemap
        REM note: level should be loaded in lua script
        REM ***************************************
        Game.world = New Level(Game, CInt(Int(Me.Width / 32)), CInt(Int(Me.Height / 32)), 32)

        'game.world = New Level(game, 25, 19, 32)
        Game.world.loadPalette("paletteLight.png", "paletteDark.png", 10)
        Game.world.ScrollPos = New Point(0, 0)
        Game.world.loadTilemap("default.level")

        REM ***************************************
        REM Initialize the sound Manager
        REM ***************************************
        SoundManager.Initialize()

        REM ***************************************
        REM Initialize the gamePad manager
        REM ***************************************
        '  GamePadManager.Initialize(game)

        REM ***************************************
        REM game loop
        REM ***************************************
        While Not gameover
            doUpdate()
        End While

    End Sub
    Private Sub Form1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        ' If Microsoft.Xna.Framework.Input.GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One).IsConnected Then Return

        Select Case Game.gameState
            Case Anataria.Game.GameStates.STATE_PLAYING
                Select Case (e.KeyCode)
                    Case Keys.Up, Keys.W : Game.keyState.up = True
                    Case Keys.Down, Keys.S : Game.keyState.down = True
                    Case Keys.Left, Keys.A : Game.keyState.left = True
                    Case Keys.Right, Keys.D : Game.keyState.right = True
                        'Case Keys.Enter : game.keyState.shift = True
                    Case Keys.T : Game.keyState.t = True
                End Select
            Case Anataria.Game.GameStates.STATE_TITLE
                Select Case e.KeyCode
                    Case Keys.Down
                        Game.titleScreen.keyState.down = True
                    Case Keys.Up
                        Game.titleScreen.keyState.up = True
                    Case Keys.Right
                        Game.titleScreen.keyState.right = True
                    Case Keys.Left
                        Game.titleScreen.keyState.left = True
                    Case Keys.Enter
                        Game.titleScreen.keyState.enter = True
                End Select
        End Select
    End Sub
    Private Sub Form1_KeyUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyUp
        'If Microsoft.Xna.Framework.Input.GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One).IsConnected Then Return

        REM state-specific key handling
        Select Case Game.gameState
            Case Game.GameStates.STATE_CHARACTER
                Select Case (e.KeyCode)

                End Select
            Case Game.GameStates.STATE_PLAYING
                Select Case (e.KeyCode)
                    Case Keys.Up, Keys.W : Game.keyState.up = False
                    Case Keys.Down, Keys.S : Game.keyState.down = False
                    Case Keys.Left, Keys.A : Game.keyState.left = False
                    Case Keys.Right, Keys.D : Game.keyState.right = False
                    Case Keys.I : Game.inven.Visible = Not Game.inven.Visible
                    Case Keys.Q : Game.quests.Visible = Not Game.quests.Visible
                    Case Keys.Space
                        lootFlag = True
                        Game.ChestFlag = True
                        Game.doorFlag = True
                        Game.switchFlag = True
                    Case Keys.Enter
                        If Game.hero.Alive Then Game.keyState.shift = True
                    Case Keys.T : Game.keyState.t = False
                    Case Keys.P : game.gameState = Anataria.Game.GameStates.STATE_PAUSED
                    Case Keys.D1, Keys.NumPad1 : game.gameState = Anataria.Game.GameStates.STATE_STATS
                End Select
            Case Anataria.Game.GameStates.STATE_PAUSED
                Select Case e.KeyCode
                    Case Keys.P : game.gameState = Anataria.Game.GameStates.STATE_PLAYING
                End Select
            Case Anataria.Game.GameStates.STATE_STATS
                Select Case e.KeyCode
                    Case Keys.D1, Keys.NumPad1 : game.gameState = Anataria.Game.GameStates.STATE_PLAYING
                End Select
            Case Anataria.Game.GameStates.STATE_TITLE
                Select Case e.KeyCode
                    Case Keys.Down
                        Game.titleScreen.keyState.down = False
                    Case Keys.Up
                        Game.titleScreen.keyState.up = False
                    Case Keys.Right
                        Game.titleScreen.keyState.right = False
                    Case Keys.Left
                        Game.titleScreen.keyState.left = False
                    Case Keys.Enter
                        Game.titleScreen.keyState.enter = False
                End Select
        End Select

        REM key handling for all states 
        Select Case (e.KeyCode)
            Case Keys.Escape
                REM handle level specific teleports
                Select Case Game.world.CurrentLevel
                    Case "Silent Temple F1.level"
                        TeleportPlayer(62, 126)
                    Case "Silent Temple F2.level"
                        TeleportPlayer(64, 14)
                End Select
                Game.hero.SaveGame(Game.savegamefile)
                game.gameState = Anataria.Game.GameStates.STATE_TITLE
            Case Keys.Divide
                ShowCommandDialog()
            Case Keys.F2 : Game.p_surface.Save("screenshot.png")
        End Select
    End Sub
    Public Sub doUpdate()
        FrameRate = Game.FrameRate()
        Dim ticks As Integer = Environment.TickCount
        Static drawLast As Integer = 0
        If ticks > drawLast + 16 Then
            drawLast = ticks

            '   GamePadManager.UpdatePad()

            Game.Device.Clear(Color.Black)
            Select Case Game.gameState
                Case Anataria.Game.GameStates.STATE_PAUSED
                    doDraw()
                    Dim sz As SizeF = Game.Device.MeasureString("Paused", font24)
                    Game.Device.DrawString("Paused", font24, Brushes.White, New Point(((Me.Width / 2) - (sz.Width / 2)), ((Me.Height / 2) - (sz.Height / 2))))
                Case Anataria.Game.GameStates.STATE_STATS
                    doDraw()
                    Game.stats.Draw()
                Case Game.GameStates.STATE_TITLE
                    doTitle()
                Case Game.GameStates.STATE_CHARACTER
                    Game.charBuilder.Draw()
                Case Game.GameStates.STATE_PLAYING
                    doScrolling()
                    doPrint()
                    doTreasure()
                    doPlayerShots()
                    doEnemyShots()
                    doAnimals()
                    doMonsters()
                    doEventMonsters()
                    doAttacks()
                    doNPCs()
                    doHero()
                    doParticles()
                    doDayandNight()
                    DrawStats()
                    doQuests()
                    doDialogue()
                    doInventory()
                    doFadingText()
                    Game.Print(0, 0, FrameRate.ToString)
            End Select
            Game.Update()
            Application.DoEvents()
        Else
            Game.world.Update()
            Threading.Thread.Sleep(1)
        End If
    End Sub
    Private Sub doDayandNight()
        Select Case My.Computer.Clock.LocalTime.Hour
            Case 1, 2, 3, 4, 5, 22, 21, 23, 24
                Dim alpha As New Pen(Color.FromArgb(255 * 0.1, 0, 0, 0))
                Game.Device.FillRectangle(alpha.Brush, 0, 0, 800, 600)
        End Select
    End Sub
    Private Sub doNPCs()
        If Game.tutorialKnight1 IsNot Nothing Then
            doTutorialKnight1()
        End If
        If Game.tutorialKnight2 IsNot Nothing Then
            doTutorialKnight2()
        End If
        If Game.Vendor IsNot Nothing Then
            doVendor()
        End If
        If Game.BlueKnight IsNot Nothing Then
            doBlueKnight()
        End If
        If Game.Peasant IsNot Nothing Then
            doPeasant()
            'game.Peasant.Update(dialog)
            '   game.Peasant.DrawToScreen()
        End If
        If Game.Urix IsNot Nothing Then
            doUrix()
        End If
    End Sub
    Private Sub doTitle()
        StopGameOver()
        PlayTheme()
        Game.titleScreen.doTitle()
        ' game.Device.Clear(Color.Black)
        ' game.Device.DrawString("MAIN MENU", font36, Brushes.Green, 290, 10)
        ' game.Device.DrawString("1 - Create Character", font24, Brushes.DarkViolet, 280, 120)
        '  game.Device.DrawString("2 - Continue Game", font24, Brushes.DarkViolet, 280, 160)
        ' game.Device.DrawString("3 - Quit", font24, Brushes.DarkViolet, 280, 200)
    End Sub
    Private Sub doScrolling()
        REM skip if inventory is open
        'If game.inven.Visible Then Return

        If Game.hero.Health <= 0 Then
            Game.keyState.shift = False
            Game.keyState.right = False
            Game.keyState.left = False
            Game.keyState.up = False
            Game.keyState.down = False
            Game.keyState.t = False
        End If

        Game.world.Draw(0, 0, Me.Width, Me.Height)
    End Sub
    Private Sub doHero()
        If Game.inven.Visible = True Then Return

        If Game.hero.AnimationState = Character.AnimationStates.Dying AndAlso Game.hero.GetSprite.CurrentFrame = (Game.hero.Direction * Game.hero.GetSprite.Columns) + Game.hero.GetSprite.Columns - 1 Then
            Game.hero.AnimationState = Character.AnimationStates.Dead
        ElseIf Game.hero.AnimationState = Character.AnimationStates.Attacking AndAlso Game.hero.GetSprite.CurrentFrame = (Game.hero.Direction * Game.hero.GetSprite.Columns) + Game.hero.GetSprite.Columns - 1 Then
            Game.keyState.shift = False
            Game.hero.AnimationState = Character.AnimationStates.Standing
        End If

        Game.hero.UpdateAttackTimer()

        REM limit player sprite to the screen boundary
        If Game.hero.X < -32 Then
            Game.hero.X = -32
        ElseIf Game.hero.X > 800 - 65 Then
            Game.hero.X = 800 - 65
        End If
        If Game.hero.Y < -48 Then
            Game.hero.Y = -48
        ElseIf Game.hero.Y > 600 - 81 Then
            Game.hero.Y = 600 - 81
        End If


        REM only run code if hero is not frozen
        If (Game.hero.Frozen) Or Not (Game.hero.Alive) Then
            Game.keyState.left = False
            Game.keyState.right = False
            Game.keyState.up = False
            Game.keyState.down = False
            Game.keyState.shift = False
        End If
        REM orient the player in the right direction
        If Not Game.hero.Alive Then
            If Game.hero.AnimationState <> Character.AnimationStates.Dying Then
                Game.hero.AnimationState = Character.AnimationStates.Dead
            Else
                Game.hero.AnimationState = Character.AnimationStates.Dying
            End If
        Else
            If Game.hero.AnimationState <> Character.AnimationStates.Attacking Then
                If Game.keyState.up And Game.keyState.right Then
                    Game.hero.Direction = 2 'north east
                    Game.hero.AnimationState = Character.AnimationStates.Walking
                ElseIf Game.keyState.right And Game.keyState.down Then
                    Game.hero.Direction = 5 'south east
                ElseIf Game.keyState.left And Game.keyState.up Then
                    Game.hero.Direction = 3 'north west
                    Game.hero.AnimationState = Character.AnimationStates.Walking
                ElseIf Game.keyState.left And Game.keyState.down Then
                    Game.hero.Direction = 6 'south west
                    Game.hero.AnimationState = Character.AnimationStates.Walking
                ElseIf Game.keyState.up Then
                    Game.hero.Direction = 1 'north
                    Game.hero.AnimationState = Character.AnimationStates.Walking
                ElseIf Game.keyState.left Then
                    Game.hero.Direction = 7 'west
                    Game.hero.AnimationState = Character.AnimationStates.Walking
                ElseIf Game.keyState.right Then
                    Game.hero.Direction = 0 'east
                    Game.hero.AnimationState = Character.AnimationStates.Walking
                ElseIf Game.keyState.down Then
                    Game.hero.Direction = 4 'south
                    Game.hero.AnimationState = Character.AnimationStates.Walking
                Else
                    If Game.hero.AnimationState <> Character.AnimationStates.Attacking And Game.hero.AnimationState <> Character.AnimationStates.Talking Then
                        Game.hero.AnimationState = Character.AnimationStates.Standing
                    End If
                End If
            End If

            If Game.keyState.shift = True Then
                Game.hero.AttackFlag = True
                If Game.hero.PlayerClass = "Hunter" Or Game.hero.PlayerClass = "Priest" Then
                    If Game.hero.AttackFlag Then
                        If Game.hero.CanAttack() Then
                            Game.hero.StartAttackTimer()
                            FirePlayerShot()
                        End If
                    End If
                End If
            End If

            If Game.hero.AttackFlag = True Then
                Game.hero.AttackFlag = False
                Game.hero.AnimationState = Character.AnimationStates.Attacking
                If Game.keyState.shift = True Then
                    If (Game.combat.Target IsNot Nothing) AndAlso (Game.combat.Target.Alive = True) And (Game.hero.Alive = True) Then
                        If Not Game.hero.PlayerClass = "Hunter" And Not Game.hero.PlayerClass = "Priest" Then
                            If Game.hero.CanAttack() Then
                                Game.hero.StartAttackTimer()
                                If Game.Distance(New PointF(Game.combat.Target.FootPos.X - Game.world.ScrollPos.X, Game.combat.Target.FootPos.Y - Game.world.ScrollPos.Y), Game.hero.FootPos) <= 70 Then
                                    Game.combat.PlayerAttack()
                                    PlaySword()
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        End If

        REM draw the hero
        Game.hero.Draw()

        If Not Game.hero.Alive Then
            If Game.hero.AnimationState <> Character.AnimationStates.Dead Then Game.hero.AnimationState = Character.AnimationStates.Dying
            PlayGameOver()
            Dim sz As SizeF = Game.Device.MeasureString("You have died", New Font("Arial", 20))
            Game.AddFadingText(Game.hero.GetSprite.X + (Game.hero.GetSprite().Width / 2) - (sz.Width / 2), Game.hero.GetSprite.Y - 40, _
            "You have died", 0.5F, Color.White, New Font("Arial", 20), True, New PointF(0, 0))
        Else
            StopGameOver()
        End If

        REM ******************************************
        REM update the stat effects
        REM ******************************************
        Game.hero.UpdateTiles()
        Game.hero.UpdateStats()

        REM heal the player over time
        Game.hero.Health += (Game.hero.HitPoints * 0.002)
        If (Game.hero.Health > Game.hero.HitPoints) Then
            Game.hero.Health = Game.hero.HitPoints
        End If
    End Sub
    Private Sub doTreasure()
        If Game.inven.Visible = True Then Return
        If Game.hero.Alive = False Then Return

        Dim relativePos As PointF
        Const lootRadius As Integer = 40
        Dim heroCenter As PointF = Game.hero.CenterPos
        Dim itemCenter As PointF
        Dim dist As Single

        For Each it In Game.treasure
            REM is item in view?
            If it.sprite.X > Game.world.ScrollPos.X - 64 _
            And it.sprite.X < Game.world.ScrollPos.X + 23 * 32 + 64 _
            And it.sprite.Y > Game.world.ScrollPos.Y - 64 _
            And it.sprite.Y < Game.world.ScrollPos.Y + 17 * 32 + 64 Then

                REM get relative position of item on screen
                relativePos.X = it.sprite.X - Game.world.ScrollPos.X
                relativePos.Y = it.sprite.Y - Game.world.ScrollPos.Y

                REM get center of item
                itemCenter = relativePos
                itemCenter.X += it.sprite.Width / 2
                itemCenter.Y += it.sprite.Height / 2

                REM get distance to the item
                dist = Game.hero.CenterDistance(itemCenter)

                REM is player trying to pick up this loot?
                If dist < lootRadius Then

                    Game.Device.DrawEllipse(New Pen(Color.Magenta, 2.0), _
                        itemCenter.X - it.sprite.Width \ 2, _
                        itemCenter.Y - it.sprite.Height \ 2, _
                        it.sprite.Width, it.sprite.Height)

                    If lootFlag Then
                        REM collect gold or item
                        If it.item.Name = "gold" Or it.item.Name = "Gold" Then
                            Game.hero.Gold += it.item.Value
                            PlayCoin()
                            Game.treasure.Remove(it)
                            Dim sz As SizeF = Game.Device.MeasureString(it.item.Value.ToString + " Gold", New Font("Narkisim", 12))
                            Game.AddFadingText(Game.hero.GetSprite.X + (Game.hero.GetSprite().Width / 2) - (sz.Width / 2), Game.hero.GetSprite.Y - 40, _
                            it.item.Value.ToString + " Gold", 0.5F, Color.White, New Font("Narkisim", 12), True, New PointF(0, -2.0F))
                            lootDialog.NumButtons = 1
                        Else
                            If Game.inven.AddItem(it.item) Then
                                PlayLootSound()
                                Game.treasure.Remove(it)
                                lootDialog.NumButtons = 1
                                Dim sz As SizeF = Game.Device.MeasureString(it.item.Name, New Font("Narkisim", 12))
                                Game.AddFadingText(Game.hero.GetSprite.X + (Game.hero.GetSprite().Width / 2) - (sz.Width / 2), Game.hero.GetSprite.Y - 40, _
                                it.item.Name, 0.5F, Color.White, New Font("Narkisim", 12), True, New PointF(0, -2.0F))
                            Else
                                lootDialog.NumButtons = 1
                                Dim sz As SizeF = Game.Device.MeasureString("Your inventory is full", New Font("Narkisim", 12))
                                Game.AddFadingText(Game.hero.GetSprite.X + (Game.hero.GetSprite().Width / 2) - (sz.Width / 2), Game.hero.GetSprite.Y - 40, _
                               "Your inventory is full", 0.5F, Color.White, New Font("Narkisim", 12), True, New PointF(0, -2.0F))
                            End If
                        End If

                        REM wait for user 
                        If lootDialog.Selection = 1 Then
                            lootFlag = False
                            lootDialog.Selection = 0
                        Else
                            lootDialog.Selection = 1
                        End If
                        Exit For
                    End If
                End If

                REM draw the item sprite
                If it.item.Name = "Green Crystal" Then
                    Game.inven.greenCrystal.Animate()
                    Game.inven.greenCrystal.Draw(relativePos.X, relativePos.Y)
                Else
                    it.sprite.Draw(relativePos.X, relativePos.Y)
                End If
            End If
        Next
        lootFlag = False
    End Sub
    Private Sub doDialogue()
        If Game.inven.Visible = True Then Return
        REM general dialog
        If Game.dialog.Visible Then
            Game.dialog.updateMouse(Game.MousePos, Game.MouseButton)
            Game.dialog.Draw()
            If Game.dialog.Selection > 0 Then
                Game.dialog.Visible = False
                Game.dialog.Selection = -1
            End If
        End If

        REM loot dialog
        If lootDialog.Visible Then
            lootDialog.updateMouse(Game.MousePos, Game.MouseButton)
            lootDialog.Draw()
            If lootDialog.Selection > 0 Then
                lootDialog.Visible = False
            End If
        End If

        REM form dialog
        If dialog.Visible Then
            dialog.updateMouse(Game.MousePos, Game.MouseButton)
            dialog.Draw()
            If dialog.Selection >= 0 Then
                Game.dialog.Selection = 0
            End If
        End If
    End Sub
    Private Sub doInventory()
        If Not Game.hero.Alive Then Game.inven.Visible = False
        If Not Game.inven.Visible Then Return
        Game.inven.updateMouse(Game.MousePos, Game.MouseButton)
        Game.inven.Draw()
    End Sub
    Private Sub doMonsters()
        If Game.inven.Visible Then Return
        Dim relativePos As PointF
        Dim attackRange As Integer = 400
        Dim attackRadius As Integer = 70
        Dim heroCenter As PointF
        Dim monsterCenter As PointF
        Dim dist As Single
        Static target As Integer = 0
        Dim index As Integer = 0
        Dim healthRect As Rectangle = New Rectangle(74, 8, 72, 8)

        heroCenter = Game.hero.CenterPos
        monstersInView = 0
        monstersInRange = 0
        index = 0

        For Each monster In Game.NPCs
            monster.UpdateAttackTimer()
            monster.Wander()
            If monster.X > Game.world.ScrollPos.X - 64 _
            And monster.X < Game.world.ScrollPos.X + 23 * 32 + 64 _
            And monster.Y > Game.world.ScrollPos.Y - 64 _
            And monster.Y < Game.world.ScrollPos.Y + 17 * 32 + 64 Then


                If monster.AnimationState = Character.AnimationStates.Dying AndAlso monster.GetSprite.CurrentFrame = (monster.Direction * monster.GetSprite.Columns) + monster.GetSprite.Columns - 1 Then
                    monster.AnimationState = Character.AnimationStates.Dead
                ElseIf monster.AnimationState = Character.AnimationStates.Attacking AndAlso monster.GetSprite.CurrentFrame = (monster.Direction * monster.GetSprite.Columns) + monster.GetSprite.Columns - 1 Then
                    monster.AnimationState = Character.AnimationStates.Standing
                End If


                index += 1
                monstersInView += 1

                relativePos.X = monster.X - Game.world.ScrollPos.X
                relativePos.Y = monster.Y - Game.world.ScrollPos.Y

                healthRect.X = relativePos.X + ((monster.GetSprite.Width / 2) - (healthRect.Width / 2))
                healthRect.Y = relativePos.Y - (healthRect.Height / 2)


                'config monster attack radius
                If monster.PlayerClass = "Hunter" Or monster.PlayerClass = "Priest" Then
                    attackRadius = 300
                    attackRange = 400
                Else
                    attackRange = 400
                    attackRadius = 70
                End If

                REM get center of NPC
                monsterCenter = relativePos
                monsterCenter.X += monster.GetSprite.Width / 2
                monsterCenter.Y += monster.GetSprite.Height / 2

                REM get distance to the NPC
                dist = Game.hero.CenterDistance(monsterCenter)

                If monster.Alive Then
                    monster.AnimationState = Character.AnimationStates.Walking
                Else
                    If monster.Name = "Green Spider" Then
                        monster.Direction = 0
                    End If
                    If monster.AnimationState <> Character.AnimationStates.Dying Then monster.AnimationState = Character.AnimationStates.Dead
                End If


                If dist < attackRange And monster.Alive = True And dist > attackRadius Then
                    monster.Chase = True
                    monster.AnimationState = Character.AnimationStates.Walking
                Else
                    monster.Chase = False
                End If


                REM is player trying to attack to this monster?
                If dist < attackRadius And monster.Alive Then
                    monstersInRange += 1
                    monster.Chase = False

                    If target > 0 Then
                        Game.Device.DrawEllipse(New Pen(Color.Red, 2.0), monsterCenter.X - 24, monsterCenter.Y, 48, 48)
                    Else
                        Game.Device.DrawEllipse(New Pen(Color.Blue, 2.0), monsterCenter.X - 24, monsterCenter.Y, 48, 48)
                    End If
                    monster.AttackFlag = True

                    ''''
                    If Game.hero.AttackFlag = True Then
                        If monster.AttackFlag = True Then
                            Game.combat.Target = monster
                        Else
                            Game.combat.Target = Nothing
                        End If
                    End If
                    ''''

                Else
                    monster.AttackFlag = False
                    If monster.Health > 0 Then monster.AnimationState = Character.AnimationStates.Walking
                    target = Nothing
                End If

                If Game.hero.Health <= 0 Then
                    monster.AnimationState = Character.AnimationStates.Walking
                    monster.Chase = False
                    monster.AttackFlag = False
                End If

                If monster.Chase Then
                    If Not monster.Frozen Then
                        monster.Moving = False
                        Dim oldPos As PointF = monster.Position
                        REM make monster face the player
                        Dim Facedir As Integer
                        Facedir = Game.combat.GetTargetDirection(monsterCenter, Game.hero.CenterPos)
                        target = index
                        monster.Direction = Facedir
                        If monster.Health > 0 Then monster.AnimationState = Character.AnimationStates.Walking
                        REM make monster follow player
                        Dim steps As Integer = 2
                        Select Case monster.Direction
                            Case 1
                                monster.Y -= steps
                            Case 2
                                monster.X += steps * 0.7
                                monster.Y -= steps * 0.7
                            Case 0
                                monster.X += steps
                            Case 5
                                monster.X += steps * 0.7
                                monster.Y += steps * 0.7
                            Case 4
                                monster.Y += steps
                            Case 6
                                monster.X -= steps * 0.7
                                monster.Y += steps * 0.7
                            Case 7
                                monster.X -= steps
                            Case 3
                                monster.X -= steps * 0.7
                                monster.Y -= steps * 0.7
                        End Select
                        REM resolve collidable tile
                        Dim tile As Level.tilemapStruct
                        Dim pos As Point = monster.GetCurrentTilePos
                        tile = Game.world.GetTile(pos.X, pos.Y)
                        If (tile.collidable) Then
                            monster.Position = oldPos
                            monster.Direction = (monster.Direction + 7) Mod 8
                            If monster.Direction < 1 Then monster.Direction = 0 - monster.Direction
                        ElseIf tile.tempCollidableForMonsters Then
                            monster.Position = oldPos
                        End If
                    Else
                        monster.AnimationState = Character.AnimationStates.Standing
                    End If
                Else
                    If monster.AttackFlag = False Then
                        monster.Moving = True
                    End If
                    target = Nothing
                End If

                REM draw the monster sprite
                If Not monster.Frozen Then
                    If monster.AttackFlag Then
                        monster.Chase = False
                        REM make monster face the player
                        Dim Facedir As Integer
                        Facedir = Game.combat.GetTargetDirection(monsterCenter, Game.hero.CenterPos)
                        target = index
                        monster.Direction = Facedir
                        If monster.Health > 0 Then
                            monster.AnimationState = Character.AnimationStates.Attacking
                        Else
                            If monster.AnimationState <> Character.AnimationStates.Dead Then
                                monster.AnimationState = Character.AnimationStates.Dying
                            Else
                                monster.AnimationState = Character.AnimationStates.Dead
                            End If
                        End If
                    End If
                Else
                    monster.AnimationState = Character.AnimationStates.Standing
                End If

                If monster.Health > 0 Then
                    Dim percent As Single = monster.Health / monster.HitPoints
                    Game.Device.DrawImage(healthBGBar, healthRect)
                    Game.Device.DrawImage(healthGaugue, healthRect.X + 1, healthRect.Y + 1, healthRect.Width * percent, healthRect.Height)
                End If
                monster.Draw(relativePos)
            End If


            REM ***********************
            REM update the stat effects
            REM ***********************
            monster.UpdateStats()

        Next

    End Sub
    Private Sub doAttacks()
        For Each monster In Game.NPCs
            If monster.AttackFlag And monster.CanAttack Then
                monster.AttackFlag = False
                monster.StartAttackTimer()
                monster.Chase = False
                If Not monster.Alive Then
                    Continue For
                End If
                Game.combat.Target = monster
                If Not monster.Frozen And monster.Health > 0 Then

                    REM special monster abillities
                    If monster.Name = "" Then
                    Else
                        If monster.PlayerClass = "Hunter" Or monster.PlayerClass = "Priest" Then
                            Dim dir As Integer = Game.combat.GetTargetDirection(monster.FootPos, Game.hero.FootPos)
                            monster.Direction = dir
                            FireEnemyShot(monster, dir)
                        Else
                            Game.combat.EnemyAttack()
                        End If
                    End If
                End If
            End If
        Next


        For Each monster In Game.eventMonsters
            If monster.AttackFlag And monster.CanAttack Then
                monster.AttackFlag = False
                monster.StartAttackTimer()
                monster.Chase = False
                If Not monster.Alive Then
                    Continue For
                End If
                Game.combat.Target = monster
                If Not monster.Frozen And monster.Health > 0 Then
                    monster.AnimationState = Character.AnimationStates.Attacking

                    REM special monster abillities
                    If monster.Name = "" Then
                    Else
                        If monster.PlayerClass = "Hunter" Or monster.PlayerClass = "Priest" Then
                            Dim dir As Integer = Game.combat.GetTargetDirection(monster.FootPos, Game.hero.FootPos)
                            monster.Direction = dir
                            FireEnemyShot(monster, dir)
                        Else
                            Game.combat.EnemyAttack()
                        End If
                    End If
                End If
            End If
        Next
    End Sub
    Public Sub doQuests()
        Game.quests.updateMouse(Game.MousePos, Game.MouseButton)
        If Not Game.inven.Visible Then Game.quests.Draw()
        If Game.quests.QuestComplete Then
            Game.quests.Visible = True
            If Game.quests.Selection > 0 Then
                Game.quests.Visible = False
                Game.quests.Selection = -1
                Dim q As Quest = Game.quests.QuestItem()
                If q.RewardXP > 0 Then
                    Game.hero.Experience += q.RewardXP
                End If
                If q.RewardGold > 0 Then
                    Game.hero.Gold += q.RewardGold
                End If
                If q.RewardItem <> "" Then
                    Game.inven.AddItem(Game.items.GetItem(q.RewardItem))
                End If
                Select Case Game.quests.QuestNumber
                    Case 0
                        Game.inven.RemoveAllItems(Game.items.GetItem("Gold Key"))
                    Case 14
                        Game.inven.RemoveAllItems(Game.items.GetItem("Green Key"))
                End Select
                Game.quests.QuestNumber += 1
                Game.quests.killCount = 0
                REM drop quest specific items and perform quest specific events
                CheckQuests()
                REM save current progress
                Game.hero.SaveGame(Game.savegamefile)
            End If
        End If
    End Sub
    Public Sub doPrint()
        Dim text As String
        doQuests()
        text = "Current quest: " + Game.quests.QuestItem.Summary
        If Game.quests.QuestComplete = True Then
            text = text + " (COMPLETE)"
        Else
            text = text + " (incomplete)"
        End If
        Game.LuaWrite(20, 77, text)
        If Game.quests.QuestItem.RequiredKillNPCFlag Then
            text = "Kills: " + Game.combat.killcount.ToString + "/" + Game.quests.QuestItem.RequiredKillCount.ToString
            Game.LuaWrite(20, 97, text)
        End If
    End Sub
    Private Sub doFadingText()
        If Game.inven.Visible Then Return
        For x As Integer = Game.fadingTexts.Count - 1 To 0 Step -1
            Game.fadingTexts(x).Draw()
            If Game.fadingTexts(x).Expired Then Game.fadingTexts.RemoveAt(x)
        Next
    End Sub
    Public Sub doDraw()
        Dim relativePos As New Point(0, 0)
        Game.world.Draw(0, 0, Me.Width, Me.Height)
        If Game.BlueKnight IsNot Nothing Then
            If Game.BlueKnight.X > Game.world.ScrollPos.X - 64 _
            And Game.BlueKnight.X < Game.world.ScrollPos.X + 23 * 32 + 64 _
            And Game.BlueKnight.Y > Game.world.ScrollPos.Y - 54 _
            And Game.BlueKnight.Y < Game.world.ScrollPos.Y + 17 * 32 Then
                relativePos.X = Game.BlueKnight.X - Game.world.ScrollPos.X
                relativePos.Y = Game.BlueKnight.Y - Game.world.ScrollPos.Y
                Game.BlueKnight.Draw(relativePos)
            End If
        End If
        If Game.tutorialKnight1 IsNot Nothing Then
            If Game.tutorialKnight1.X > Game.world.ScrollPos.X - 64 _
            And Game.tutorialKnight1.X < Game.world.ScrollPos.X + 23 * 32 + 64 _
            And Game.tutorialKnight1.Y > Game.world.ScrollPos.Y - 54 _
            And Game.tutorialKnight1.Y < Game.world.ScrollPos.Y + 17 * 32 Then
                relativePos.X = Game.tutorialKnight1.X - Game.world.ScrollPos.X
                relativePos.Y = Game.tutorialKnight1.Y - Game.world.ScrollPos.Y
                Game.tutorialKnight1.Draw(relativePos)
            End If
        End If
        If Game.tutorialKnight2 IsNot Nothing Then
            If Game.tutorialKnight2.X > Game.world.ScrollPos.X - 64 _
            And Game.tutorialKnight2.X < Game.world.ScrollPos.X + 23 * 32 + 64 _
            And Game.tutorialKnight2.Y > Game.world.ScrollPos.Y - 54 _
            And Game.tutorialKnight2.Y < Game.world.ScrollPos.Y + 17 * 32 Then
                relativePos.X = Game.tutorialKnight2.X - Game.world.ScrollPos.X
                relativePos.Y = Game.tutorialKnight2.Y - Game.world.ScrollPos.Y
                Game.tutorialKnight2.Draw(relativePos)
            End If
        End If
        If Game.Peasant IsNot Nothing Then
            If Game.Peasant.X > Game.world.ScrollPos.X - 64 _
         And Game.Peasant.X < Game.world.ScrollPos.X + 23 * 32 + 64 _
         And Game.Peasant.Y > Game.world.ScrollPos.Y - 54 _
         And Game.Peasant.Y < Game.world.ScrollPos.Y + 17 * 32 Then
                relativePos.X = Game.Peasant.X - Game.world.ScrollPos.X
                relativePos.Y = Game.Peasant.Y - Game.world.ScrollPos.Y
                Game.Peasant.Draw(relativePos)
            End If
        End If
        If Game.Vendor IsNot Nothing Then
            If Game.Vendor.X > Game.world.ScrollPos.X - 64 _
         And Game.Vendor.X < Game.world.ScrollPos.X + 23 * 32 + 64 _
         And Game.Vendor.Y > Game.world.ScrollPos.Y - 54 _
         And Game.Vendor.Y < Game.world.ScrollPos.Y + 17 * 32 Then
                relativePos.X = Game.Vendor.X - Game.world.ScrollPos.X
                relativePos.Y = Game.Vendor.Y - Game.world.ScrollPos.Y
                Game.Vendor.Draw(relativePos)
            End If
        End If
        Game.hero.AnimationState = Character.AnimationStates.Standing
        Game.hero.Draw()
        For Each monster In Game.NPCs
            If monster.X > Game.world.ScrollPos.X - 64 _
            And monster.X < Game.world.ScrollPos.X + 23 * 32 + 64 _
            And monster.Y > Game.world.ScrollPos.Y - 64 _
            And monster.Y < Game.world.ScrollPos.Y + 17 * 32 + 64 Then
                relativePos.X = monster.X - Game.world.ScrollPos.X
                relativePos.Y = monster.Y - Game.world.ScrollPos.Y
                monster.Draw(relativePos)
            End If
        Next
        For Each monster As Character In Game.eventMonsters
            relativePos.X = monster.X - Game.world.ScrollPos.X
            relativePos.Y = monster.Y - Game.world.ScrollPos.Y
            monster.Draw(relativePos)
        Next
        For Each it In Game.treasure
            REM is item in view?
            If it.sprite.X > Game.world.ScrollPos.X - 64 _
            And it.sprite.X < Game.world.ScrollPos.X + 23 * 32 + 64 _
            And it.sprite.Y > Game.world.ScrollPos.Y - 64 _
            And it.sprite.Y < Game.world.ScrollPos.Y + 17 * 32 + 64 Then
                relativePos.X = it.sprite.X - Game.world.ScrollPos.X
                relativePos.Y = it.sprite.Y - Game.world.ScrollPos.Y
                If it.item.Name = "Green Crystal" Then
                    Game.inven.greenCrystal.Animate()
                    Game.inven.greenCrystal.Draw(relativePos.X, relativePos.Y)
                Else
                    it.sprite.Draw(relativePos.X, relativePos.Y)
                End If
            End If
        Next
        ' If Not game.inven.Visible Then game.quests.Draw()
        ' game.inven.Draw()
        ' game.dialog.Draw()
        ' lootDialog.Draw()
        ' dialog.Draw()
    End Sub
    Public Sub RespawnHero()
        Game.hero.Alive = True
        Game.hero.Frozen = False
        Game.hero.Bleeding = False
        Game.hero.bleedPower = 0.0F
        Game.hero._bleedTime = 0.0F
        Game.hero._freezeTime = 0.0F
        Game.hero.Regenerating = False
        Game.hero._regenTime = 0.0F
        Game.hero.regenPower = 0.0F
        Game.hero.Health = Game.hero.HitPoints
        Game.hero.AnimationState = Character.AnimationStates.Standing
        TeleportPlayer(Game.world.LastSpawn.X, Game.world.LastSpawn.Y)
    End Sub
    Public Sub doAnimals()
        For Each Animal As Animal In Game.animals
            Animal.Update()
            Animal.DrawToScreen()
        Next
    End Sub
    Public Sub doEventMonsters()
        If Game.inven.Visible Then Return
        Dim relativePos As PointF
        Dim attackRange As Integer = 400
        Dim attackRadius As Integer = 70
        Dim heroCenter As PointF
        Dim monsterCenter As PointF
        Dim dist As Single
        Static target As Integer = 0
        Dim index As Integer = 0
        Dim healthRect As Rectangle = New Rectangle(74, 8, 72, 8)

        heroCenter = Game.hero.CenterPos
        monstersInView = 0
        monstersInRange = 0
        index = 0

        For Each monster In Game.eventMonsters
            monster.UpdateAttackTimer()
            monster.Wander()
            If monster.X > Game.world.ScrollPos.X - 64 _
            And monster.X < Game.world.ScrollPos.X + 23 * 32 + 64 _
            And monster.Y > Game.world.ScrollPos.Y - 64 _
            And monster.Y < Game.world.ScrollPos.Y + 17 * 32 + 64 Then

                If monster.AnimationState = Character.AnimationStates.Dying AndAlso monster.GetSprite.CurrentFrame = (monster.Direction * monster.GetSprite.Columns) + monster.GetSprite.Columns - 1 Then
                    monster.AnimationState = Character.AnimationStates.Dead
                ElseIf monster.AnimationState = Character.AnimationStates.Attacking AndAlso monster.GetSprite.CurrentFrame = (monster.Direction * monster.GetSprite.Columns) + monster.GetSprite.Columns - 1 Then
                    monster.AnimationState = Character.AnimationStates.Standing
                End If

                '  game.AddFadingText(monster.X - game.world.X, monster.Y - game.world.Y, game.eventMonsters.IndexOf(monster).ToString, 0.1F)

                index += 1
                monstersInView += 1

                relativePos.X = monster.X - Game.world.ScrollPos.X
                relativePos.Y = monster.Y - Game.world.ScrollPos.Y

                healthRect.X = relativePos.X + ((monster.GetSprite.Width / 2) - (healthRect.Width / 2))
                healthRect.Y = relativePos.Y - (healthRect.Height / 2)


                'config monster attack radius
                If monster.PlayerClass = "Hunter" Or monster.PlayerClass = "Priest" Then
                    attackRadius = 300
                    attackRange = 400
                Else
                    attackRange = 400
                    attackRadius = 70
                End If

                If monster.Name = "Red Spider" Then
                    attackRadius = 300
                End If

                REM get center of NPC
                monsterCenter = relativePos
                monsterCenter.X += monster.GetSprite.Width / 2
                monsterCenter.Y += monster.GetSprite.Height / 2

                REM get distance to the NPC
                dist = Game.hero.CenterDistance(monsterCenter)

                If monster.Alive Then
                    monster.AnimationState = Character.AnimationStates.Walking
                Else
                    If monster.Name = "Green Spider" Then
                        monster.Direction = 0
                    End If
                    If monster.AnimationState <> Character.AnimationStates.Dying Then monster.AnimationState = Character.AnimationStates.Dead
                End If


                If dist < attackRange And monster.Alive = True And dist > attackRadius Then
                    monster.Chase = True
                    monster.AnimationState = Character.AnimationStates.Walking
                Else
                    monster.Chase = False
                End If


                REM is player trying to attack to this monster?
                If dist < attackRadius And monster.Alive Then
                    monstersInRange += 1
                    monster.Chase = False

                    If target > 0 Then
                        Game.Device.DrawEllipse(New Pen(Color.Red, 2.0), monsterCenter.X - 24, monsterCenter.Y, 48, 48)
                    Else
                        Game.Device.DrawEllipse(New Pen(Color.Blue, 2.0), monsterCenter.X - 24, monsterCenter.Y, 48, 48)
                    End If
                    monster.AttackFlag = True

                    ''''
                    If Game.hero.AttackFlag = True Then
                        If monster.AttackFlag = True Then
                            Game.combat.Target = monster
                        Else
                            Game.combat.Target = Nothing
                        End If
                    End If
                    ''''

                Else
                    monster.AttackFlag = False
                    If monster.Health > 0 Then monster.AnimationState = Character.AnimationStates.Walking
                    target = Nothing
                End If

                If Game.hero.Health <= 0 Then
                    monster.AnimationState = Character.AnimationStates.Walking
                    monster.Chase = False
                    monster.AttackFlag = False
                End If

                If monster.Chase Then
                    If Not monster.Frozen Then
                        monster.Moving = False
                        Dim oldPos As PointF = monster.Position
                        REM make monster face the player
                        Dim Facedir As Integer
                        Facedir = Game.combat.GetTargetDirection(monsterCenter, Game.hero.CenterPos)
                        target = index
                        monster.Direction = Facedir
                        If monster.Health > 0 Then monster.AnimationState = Character.AnimationStates.Walking
                        REM make monster follow player
                        Dim steps As Integer = 2
                        Select Case monster.Direction
                            Case 1
                                monster.Y -= steps
                            Case 2
                                monster.X += steps * 0.7
                                monster.Y -= steps * 0.7
                            Case 0
                                monster.X += steps
                            Case 5
                                monster.X += steps * 0.7
                                monster.Y += steps * 0.7
                            Case 4
                                monster.Y += steps
                            Case 6
                                monster.X -= steps * 0.7
                                monster.Y += steps * 0.7
                            Case 7
                                monster.X -= steps
                            Case 3
                                monster.X -= steps * 0.7
                                monster.Y -= steps * 0.7
                        End Select
                        REM resolve collidable tile
                        Dim tile As Level.tilemapStruct
                        Dim pos As Point = monster.GetCurrentTilePos
                        tile = Game.world.GetTile(pos.X, pos.Y)
                        If (tile.collidable) Then
                            monster.Position = oldPos
                            monster.Direction = (monster.Direction + 7) Mod 8
                            If monster.Direction < 1 Then monster.Direction = 0 - monster.Direction
                        ElseIf tile.tempCollidableForMonsters Then
                            monster.Position = oldPos
                        End If
                    Else
                        monster.AnimationState = Character.AnimationStates.Standing
                    End If
                Else
                    If monster.AttackFlag = False Then
                        monster.Moving = True
                    End If
                    target = Nothing
                End If

                REM draw the monster sprite
                If Not monster.Frozen Then
                    If monster.AttackFlag Then
                        monster.Chase = False
                        REM make monster face the player
                        Dim Facedir As Integer
                        Facedir = Game.combat.GetTargetDirection(monsterCenter, Game.hero.CenterPos)
                        target = index
                        monster.Direction = Facedir
                        If monster.Health > 0 Then
                            monster.AnimationState = Character.AnimationStates.Attacking
                        Else
                            If monster.AnimationState <> Character.AnimationStates.Dead Then
                                monster.AnimationState = Character.AnimationStates.Dying
                            Else
                                monster.AnimationState = Character.AnimationStates.Dead
                            End If
                        End If
                    End If
                Else
                    monster.AnimationState = Character.AnimationStates.Standing
                End If
                If monster.Health > 0 Then
                    Dim percent As Single = monster.Health / monster.HitPoints
                    Game.Device.DrawImage(healthBGBar, healthRect)
                    Game.Device.DrawImage(healthGaugue, healthRect.X + 1, healthRect.Y + 1, healthRect.Width * percent, healthRect.Height)
                End If
                monster.Draw(relativePos)
            End If


            REM ***********************
            REM update the stat effects
            REM ***********************
            monster.UpdateStats()

        Next

    End Sub
    Public Sub PauseProgram(seconds As Single)
        Game.Update()
        Application.DoEvents()
        Threading.Thread.Sleep(seconds * 1000)
    End Sub
    Public Sub TeleportPlayer(portalX As Integer, portalY As Integer)
        Dim p_portalTarget As New Point(portalX, portalY)
        Game.hero.SetFootPos(p_portalTarget.X, p_portalTarget.Y)
        Game.world.X = Game.hero.Position.X - (400 - 48)
        Game.world.Y = Game.hero.Position.Y - (300 - 48)
        If Game.world.X < 0 Then Game.world.X = 0
        If Game.world.Y < 0 Then Game.world.Y = 0
        If Game.world.X > (128 * 32) - 800 Then Game.world.X = (128 * 32) - 800
        If Game.world.Y > (128 * 32) - 600 Then Game.world.Y = (128 * 32) - 600
        Game.hero.SetFootPos(p_portalTarget.X, p_portalTarget.Y)
        Game.hero.X -= Game.world.ScrollPos.X
        Game.hero.Y -= Game.world.ScrollPos.Y
        Game.world.LastSpawn = Game.hero.GetCurrentTilePos
    End Sub
    Public Sub TeleportMonster(ByRef npc As Character, portalX As Integer, portalY As Integer)
        Dim p_portalTarget As New Point(portalX, portalY)
        npc.SetFootPos(p_portalTarget.X, p_portalTarget.Y)
    End Sub
    Private Sub doParticles()
        For x As Integer = Game.particles.Count - 1 To 0 Step -1
            Game.particles(x).Animate()
            If Game.particles(x).Expired = False Then
                Game.particles(x).Draw()
            Else
                Game.particles.RemoveAt(x)
            End If
        Next
    End Sub
#Region "Update Character Methods"
    Public Sub doPeasant()
        Dim talkFlag As Boolean = False
        Dim relativePos As PointF
        Const talkRadius As Integer = 70
        Dim heroCenter As PointF
        Dim npcCenter As PointF
        Dim dist As Single
        Static target As Integer = 0
        Dim index As Integer = 0

        If Game.inven.Visible Then
            Return
        End If

        Game.Peasant.UpdateTiles()

        If Game.Peasant.X > Game.world.ScrollPos.X - 64 _
        And Game.Peasant.X < Game.world.ScrollPos.X + 23 * 32 + 64 _
        And Game.Peasant.Y > Game.world.ScrollPos.Y - 54 _
        And Game.Peasant.Y < Game.world.ScrollPos.Y + 17 * 32 Then
            heroCenter = Game.hero.CenterPos
            relativePos.X = Game.Peasant.X - Game.world.ScrollPos.X
            relativePos.Y = Game.Peasant.Y - Game.world.ScrollPos.Y
            Game.Peasant.Draw(relativePos)

            REM get center of NPC
            npcCenter = relativePos
            npcCenter.X += Game.Peasant.GetSprite.Width / 2
            npcCenter.Y += Game.Peasant.GetSprite.Height / 2

            REM get distance to the NPC
            dist = Game.hero.CenterDistance(npcCenter)

            REM is player trying to talk to this NPC?
            If dist < talkRadius And Game.Peasant.Alive Then
                Game.Device.DrawEllipse(New Pen(Color.Blue, 2.0), npcCenter.X - 24, npcCenter.Y, 48, 48)
                talkFlag = True
                If Game.casualVisible = False Then
                    Game.p_dialog = Game.Random(0, 4)
                End If
                Game.casualVisible = True
            Else
                Game.casualVisible = False
                talkFlag = False
            End If
        Else
            talkFlag = False
        End If

        'If game.keyState.t Then
        If talkFlag Then
            talkFlag = False
            Game.Peasant.Moving = False

            REM make monster face the player
            Dim dir As Integer
            dir = Game.combat.GetTargetDirection(npcCenter, Game.hero.CenterPos)
            If Game.Peasant.AnimationState <> Character.AnimationStates.Dead Then
                Game.Peasant.Direction = dir
                Game.Peasant.AnimationState = Character.AnimationStates.Talking
            End If

            REM make player face the NPC
            dir = Game.combat.GetTargetDirection(Game.hero.CenterPos, npcCenter)
            If Game.Peasant.AnimationState <> Character.AnimationStates.Dead Then
                Game.hero.AnimationState = Character.AnimationStates.Talking
                Game.hero.Direction = dir
            End If
            Static advance As Boolean = False
            Static questIncreased As Boolean = False
            Select Case Game.quests.QuestNumber
                Case 4
                    If Game.Peasant.AnimationState <> Character.AnimationStates.Dead Then
                        dialog.Title = "Wounded Farmer"
                        dialog.Message = "Huh? Who are you? Are you a celtic? Wha..The village? I'm not entirely sure... It " + _
                            "all happened so quick. I remember something about bandits. Ah, yes! It was bandits. They pillaged the village. We couldn't stop " + _
                            "them. We're just honest farm folk. None of us know how to fight. I hope you can catch them. I think they headed due west. " + _
                            "...I wish I could be of more use. Unh!"
                        dialog.NumButtons = 1
                        dialog.setButtonText(1, "Next")
                        dialog.setCorner(Dialogue.Positions.UpperLeft)
                        dialog.Visible = True
                        If dialog.Selection = 1 Then
                            Game.Peasant.AnimationState = Character.AnimationStates.Dead
                            dialog.Visible = False
                            Game.hero.Frozen = False
                            Game.hero.iceSprite.Image = Game.LoadBitmap("frozenmagic.png")
                            advance = True
                        Else
                            If Game.quests.QuestNumber <> 4 Then Game.quests.QuestNumber = 4
                            Game.hero.Frozen = True
                            Game.hero.iceSprite.Image = Game.LoadBitmap("blank square.png")
                            Game.hero._freezeTime = 10
                        End If
                    End If
                Case Else
                    If questIncreased Then
                        showDialogTutorialKnight1("Wounded Farmer", "He's dead. There is nothing left to see.", 1, "Okay")
                        If dialog.Selection = 1 Then
                            dialog.Visible = False
                            Game.hero.Frozen = False
                            Game.hero.iceSprite.Image = Game.LoadBitmap("frozenmagic.png")
                        Else
                            Game.hero.Frozen = True
                            Game.hero.iceSprite.Image = Game.LoadBitmap("blank square.png")
                            Game.hero._freezeTime = 10
                        End If
                    End If
            End Select
            If Not advance Then
                If Game.quests.QuestNumber <> 4 Then Game.quests.QuestNumber = 4
            Else
                If Not questIncreased Then
                    questIncreased = True
                    Game.quests.QuestNumber += 1
                End If
            End If
        Else
            If Game.Peasant.AnimationState <> Character.AnimationStates.Dead Then
                Game.Peasant.AnimationState = Character.AnimationStates.Standing
            End If
        End If
        'End If
    End Sub
    Public Sub doUrix()
        Dim talkFlag As Boolean = False
        Dim relativePos As PointF
        Const talkRadius As Integer = 70
        Dim heroCenter As PointF
        Dim npcCenter As PointF
        Dim dist As Single
        Static target As Integer = 0
        Dim index As Integer = 0

        If Game.inven.Visible Then
            Return
        End If

        Game.Urix.UpdateTiles()

        If Game.Urix.X > Game.world.ScrollPos.X - 64 _
        And Game.Urix.X < Game.world.ScrollPos.X + 23 * 32 + 64 _
        And Game.Urix.Y > Game.world.ScrollPos.Y - 54 _
        And Game.Urix.Y < Game.world.ScrollPos.Y + 17 * 32 Then
            heroCenter = Game.hero.CenterPos
            relativePos.X = Game.Urix.X - Game.world.ScrollPos.X
            relativePos.Y = Game.Urix.Y - Game.world.ScrollPos.Y
            Game.Urix.Draw(relativePos)

            REM get center of NPC
            npcCenter = relativePos
            npcCenter.X += Game.Urix.GetSprite.Width / 2
            npcCenter.Y += Game.Urix.GetSprite.Height / 2

            REM get distance to the NPC
            dist = Game.hero.CenterDistance(npcCenter)

            REM is player trying to talk to this NPC?
            If dist < talkRadius And Game.Urix.Alive Then
                Game.Device.DrawEllipse(New Pen(Color.Blue, 2.0), npcCenter.X - 24, npcCenter.Y, 48, 48)
                talkFlag = True
                If Game.casualVisible = False Then
                    Game.p_dialog = Game.Random(0, 4)
                End If
                Game.casualVisible = True
            Else
                Game.casualVisible = False
                talkFlag = False
            End If
        Else
            talkFlag = False
        End If

        'If game.keyState.t Then
        If talkFlag Then
            talkFlag = False
            Game.Urix.Moving = False

            REM make monster face the player
            Dim dir As Integer
            dir = Game.combat.GetTargetDirection(npcCenter, Game.hero.CenterPos)
            If Game.Urix.AnimationState <> Character.AnimationStates.Dead Then
                Game.Urix.Direction = dir
                Game.Urix.AnimationState = Character.AnimationStates.Talking
            End If

            REM make player face the NPC
            dir = Game.combat.GetTargetDirection(Game.hero.CenterPos, npcCenter)
            If Game.Urix.AnimationState <> Character.AnimationStates.Dead Then
                Game.hero.AnimationState = Character.AnimationStates.Talking
                Game.hero.Direction = dir
            End If
            Static nextClicked As Boolean = False
            Static nextClicked2 As Boolean = False
            Static nextClicked3 As Boolean = False
            Static CloseClicked As Boolean = False
            Static Increased As Boolean = False
            Select Case Game.quests.QuestNumber
                Case 6
                    dialog.Title = "Urix"
                    If Not nextClicked Then
                        dialog.Message = "Hello, " + Game.hero.Name + ". My name is Urix. I am the head monk of the East Sea Monastary. You need not tell me why you are here for I already know. " + _
                            "I will answer your questions. As you know, creatures that are not of this world are in Vatura. Several months ago, these strange creatures started to appear. The creatures seem to have originated near the middle " + _
                            "of Vatura."
                    ElseIf nextClicked And Not nextClicked2 Then
                        dialog.Message = "Near their origin, the sky swirls with a purple cloud. No one can get near to where the monsters are coming from. A strange magic warps you back " + _
                            "to whence you came. I suspect a dark energy is at work. I have reason to believe this because the monsters have not been seen on this island. This island " + _
                            "is holy and radiates divine energy. Dark magic fears that of the divine."
                    ElseIf nextClicked And nextClicked2 And Not nextClicked3 Then
                        dialog.Message = "If the monsters' apperances are indeed the result of dark magic, then the only way you can penetrate the dark aura that surrounds the anomaly is by collecting the " + _
                           "three sacred keys. These keys radiate holy energy. If you can obtain all three, it should be enough to part the barrier. The three keys are well protected. You will face " + _
                           "great peril in your quest to find them."
                    ElseIf nextClicked3 Then
                        dialog.Message = "The first key is located at the heart of the silent forest. The forests entrance is located directly west of the bottom portion of " + _
                          "the East Coast. If you obtain the key, return to me with it and I will tell you more."
                    End If
                    dialog.NumButtons = 1
                    dialog.setButtonText(1, "Next")
                    dialog.setCorner(Dialogue.Positions.UpperLeft)
                    dialog.Visible = True
                    If dialog.Selection = 1 Then
                        If nextClicked = False Then
                            nextClicked = True
                        ElseIf nextClicked = True AndAlso nextClicked2 = False Then
                            nextClicked2 = True
                        ElseIf nextClicked2 = True AndAlso nextClicked3 = False Then
                            nextClicked3 = True
                        ElseIf nextClicked3 = True Then
                            CloseClicked = True
                        End If
                    End If
                    If Not CloseClicked Then
                        Game.hero.Frozen = True
                        Game.hero.iceSprite.Image = Game.LoadBitmap("blank square.png")
                        Game.hero._freezeTime = 10
                    Else
                        dialog.Visible = False
                        Game.hero.Frozen = False
                        Game.hero.iceSprite.Image = Game.LoadBitmap("frozenmagic.png")
                    End If
                Case 7
                    If CloseClicked Then
                        '   If Not Increased Then
                        '       Increased = True
                        '       game.quests.QuestNumber += 1
                        '       CloseClicked = False
                        '       nextClicked = False
                        '       nextClicked2 = False
                        '       nextClicked3 = False
                        '       Me.Visible = False
                        '   End If
                    Else
                        If Game.quests.QuestNumber <> 6 Then Game.quests.QuestNumber = 6
                    End If
                Case 13
                    dialog.Title = "Urix"
                    If Not nextClicked Then
                        dialog.Message = "Oh Ho! So you are worthy of this quest. I am impressed that you recovered the first key so easily. Very well. I will tell you the rest. As I told " + _
                            "you before, you will need all three keys to bypass the dark barrier. Each of the three keys contains an essence of reality. These include Earth, Ice, and Fire. " + _
                            "The key you just obtained is Earth. Each key is also located in a temple like the"
                    ElseIf nextClicked And Not nextClicked2 Then
                        dialog.Message = "one you just found. Each temple has a ferocious gardian that holds the temple's key.You will need to defeat these gardians to obtain each temple's key. Each time you retreive a key, bring it back to me for the keys will be safe on holy ground." + _
                            " When you have all three keys, there is a sacred temple hidden in the desert. You will have to take each of the three keys"
                    ElseIf nextClicked And nextClicked2 Then
                        dialog.Message = "there to merge them into the divine key of light. Only the divine key of light can break the dark barrier. The next key you should find is the " + _
                            "Divine Key of Chaos. It's temple is hidden in the mountain to " + _
                            "the north of the Northern East Coast. "
                    End If
                    dialog.NumButtons = 1
                    dialog.setButtonText(1, "Next")
                    dialog.setCorner(Dialogue.Positions.UpperLeft)
                    dialog.Visible = True
                    If dialog.Selection = 1 Then
                        If nextClicked = False Then
                            nextClicked = True
                        ElseIf nextClicked = True AndAlso nextClicked2 = False Then
                            nextClicked2 = True
                        ElseIf nextClicked2 = True AndAlso nextClicked3 = False Then
                            CloseClicked = True
                        End If
                    End If
                    If Not CloseClicked Then
                        Game.hero.Frozen = True
                        Game.hero.iceSprite.Image = Game.LoadBitmap("blank square.png")
                        Game.hero._freezeTime = 10
                    Else
                        dialog.Visible = False
                        Game.hero.Frozen = False
                        Game.hero.iceSprite.Image = Game.LoadBitmap("frozenmagic.png")
                    End If
                Case 14
                    If CloseClicked Then
                        'If Not Increased and Then
                        'Increased = True
                        'game.quests.QuestNumber += 1
                        'CloseClicked = False
                        ' nextClicked = False
                        '  nextClicked2 = False
                        '   nextClicked3 = False
                        '  dialog.Visible = False
                        '    game.inven.RemoveAllItems(game.items.GetItem("Green Key"))
                        ' End If
                    Else
                        If Game.quests.QuestNumber <> 13 Then Game.quests.QuestNumber = 13
                    End If
                Case Else
                    '  game.CasualDialog(game.Urix)
            End Select
        Else
            If Game.Urix.AnimationState <> Character.AnimationStates.Dead Then
                Game.Urix.AnimationState = Character.AnimationStates.Standing
            End If
        End If
        'End If
    End Sub
    Public Sub doTutorialKnight1()
        Dim talkFlag As Boolean = False
        Dim relativePos As PointF
        Const talkRadius As Integer = 70
        Dim heroCenter As PointF
        Dim npcCenter As PointF
        Dim dist As Single
        Static target As Integer = 0
        Dim index As Integer = 0

        If Game.inven.Visible Then
            Return
        End If

        Game.tutorialKnight1.UpdateTiles()

        If Game.tutorialKnight1.X > Game.world.ScrollPos.X - 64 _
        And Game.tutorialKnight1.X < Game.world.ScrollPos.X + 23 * 32 + 64 _
        And Game.tutorialKnight1.Y > Game.world.ScrollPos.Y - 54 _
        And Game.tutorialKnight1.Y < Game.world.ScrollPos.Y + 17 * 32 Then
            heroCenter = Game.hero.CenterPos
            relativePos.X = Game.tutorialKnight1.X - Game.world.ScrollPos.X
            relativePos.Y = Game.tutorialKnight1.Y - Game.world.ScrollPos.Y
            Game.tutorialKnight1.Draw(relativePos)

            REM get center of NPC
            npcCenter = relativePos
            npcCenter.X += Game.tutorialKnight1.GetSprite.Width / 2
            npcCenter.Y += Game.tutorialKnight1.GetSprite.Height / 2

            REM get distance to the NPC
            dist = Game.hero.CenterDistance(npcCenter)

            REM is player trying to talk to this NPC?
            If dist < talkRadius And Game.tutorialKnight1.Alive Then
                Game.Device.DrawEllipse(New Pen(Color.Blue, 2.0), npcCenter.X - 24, npcCenter.Y, 48, 48)
                talkFlag = True
                If Game.casualVisible = False Then
                    Game.p_dialog = Game.Random(0, 4)
                End If
                Game.casualVisible = True
            Else
                Game.casualVisible = False
                talkFlag = False
            End If
        Else
            talkFlag = False
        End If

        'If game.keyState.t Then
        If talkFlag Then
            talkFlag = False
            Game.tutorialKnight1.Moving = False

            REM make monster face the player
            Dim dir As Integer
            dir = Game.combat.GetTargetDirection(npcCenter, Game.hero.CenterPos)
            Game.tutorialKnight1.Direction = dir
            Game.tutorialKnight1.AnimationState = Character.AnimationStates.Talking
            If Game.FrameRate >= 60 Then Game.tutorialKnight1.Draw(relativePos)

            REM make player face the NPC
            dir = Game.combat.GetTargetDirection(Game.hero.CenterPos, npcCenter)
            Game.hero.Direction = dir
            Game.hero.AnimationState = Character.AnimationStates.Talking
            If Game.FrameRate >= 60 Then Game.hero.Draw()
            Select Case Game.quests.QuestNumber
                Case 0, 1
                    Game.showDialogTutorialKnight1("Sir Montresoir", "Welcome brave Adventurer! This is the world of Celtic Crusader. You won't get very " _
                                    + "far without any combat trainging. Kill five skeletons and return.")
                Case 2
                    Game.showDialogTutorialKnight1("Sir Montresoir", "Great job! You are truly a gifted " + Game.hero.PlayerClass + ". You look a little worn down though. " _
                                                   + "You should travel south until you find another knight. His name is Sir Faromir. He will direct " _
                                                   + "you to the nearest town.")
                Case Else
                    Game.CasualDialog(Game.tutorialKnight1)
            End Select
        Else
            Game.tutorialKnight1.AnimationState = Character.AnimationStates.Standing
        End If
        'End If
    End Sub
    Public Sub doTutorialKnight2()
        Dim relativePos As PointF
        Const talkRadius As Integer = 70
        Dim heroCenter As PointF
        Dim npcCenter As PointF
        Dim dist As Single
        Static target As Integer = 0
        Dim index As Integer = 0

        If Game.inven.Visible Then
            Return
        End If

        Game.tutorialKnight2.UpdateTiles()

        If Game.tutorialKnight2.X > Game.world.ScrollPos.X - 64 _
        And Game.tutorialKnight2.X < Game.world.ScrollPos.X + 23 * 32 + 64 _
        And Game.tutorialKnight2.Y > Game.world.ScrollPos.Y - 54 _
        And Game.tutorialKnight2.Y < Game.world.ScrollPos.Y + 17 * 32 Then
            heroCenter = Game.hero.CenterPos
            relativePos.X = Game.tutorialKnight2.X - Game.world.ScrollPos.X
            relativePos.Y = Game.tutorialKnight2.Y - Game.world.ScrollPos.Y
            Game.tutorialKnight2.Draw(relativePos)

            REM get center of NPC
            npcCenter = relativePos
            npcCenter.X += Game.tutorialKnight2.GetSprite.Width / 2
            npcCenter.Y += Game.tutorialKnight2.GetSprite.Height / 2

            REM get distance to the NPC
            dist = Game.hero.CenterDistance(npcCenter)

            REM is player trying to talk to this NPC?
            If dist < talkRadius And Game.tutorialKnight2.Alive Then
                Game.Device.DrawEllipse(New Pen(Color.Blue, 2.0), npcCenter.X - 24, npcCenter.Y, 48, 48)
                talkFlag = True
                If Game.casualVisible = False Then
                    Game.p_dialog = Game.Random(0, 4)
                End If
                Game.casualVisible = True
            Else
                Game.casualVisible = False
                talkFlag = False
            End If
        End If


        'If game.keyState.t Then
        If talkFlag Then
            talkFlag = False
            Game.tutorialKnight2.Moving = False

            REM make monster face the player
            Dim dir As Integer
            dir = Game.combat.GetTargetDirection(npcCenter, Game.hero.CenterPos)
            Game.tutorialKnight2.Direction = dir
            Game.tutorialKnight2.AnimationState = Character.AnimationStates.Talking

            REM make player face the NPC
            dir = Game.combat.GetTargetDirection(Game.hero.CenterPos, npcCenter)
            Game.hero.Direction = dir
            Game.hero.AnimationState = Character.AnimationStates.Talking
            Select Case Game.quests.QuestNumber
                Case 1
                    Game.showDialogTutorialKnight1("Plated Knight", "Great job player! Yep, that's how it's done! If you want to talk to us, just walk on up." + _
                        " You'll know you're close enough when our circle appears around our feet. Just don't try to talk to the monsters!")
                Case Else
                    Game.CasualDialog(Game.tutorialKnight2)
            End Select
        Else
            Game.tutorialKnight2.AnimationState = Character.AnimationStates.Standing
        End If
        'End If
    End Sub
    Public Sub doBlueKnight()
        Dim relativePos As PointF
        Const talkRadius As Integer = 70
        Dim heroCenter As PointF
        Dim npcCenter As PointF
        Dim dist As Single
        Static target As Integer = 0
        Dim index As Integer = 0

        If Game.inven.Visible Then
            Return
        End If

        Game.BlueKnight.UpdateTiles()

        If Game.BlueKnight.X > Game.world.ScrollPos.X - 64 _
        And Game.BlueKnight.X < Game.world.ScrollPos.X + 23 * 32 + 64 _
        And Game.BlueKnight.Y > Game.world.ScrollPos.Y - 54 _
        And Game.BlueKnight.Y < Game.world.ScrollPos.Y + 17 * 32 Then
            heroCenter = Game.hero.CenterPos
            relativePos.X = Game.BlueKnight.X - Game.world.ScrollPos.X
            relativePos.Y = Game.BlueKnight.Y - Game.world.ScrollPos.Y
            Game.BlueKnight.Draw(relativePos)

            REM get center of NPC
            npcCenter = relativePos
            npcCenter.X += Game.BlueKnight.GetSprite.Width / 2
            npcCenter.Y += Game.BlueKnight.GetSprite.Height / 2

            REM get distance to the NPC
            dist = Game.hero.CenterDistance(npcCenter)

            REM is player trying to talk to this NPC?
            If dist < talkRadius And Game.BlueKnight.Alive Then
                Game.Device.DrawEllipse(New Pen(Color.Blue, 2.0), npcCenter.X - 24, npcCenter.Y, 48, 48)
                talkFlag = True
                If Game.casualVisible = False Then
                    Game.p_dialog = Game.Random(0, 4)
                End If
                Game.casualVisible = True
            Else
                Game.casualVisible = False
                talkFlag = False
            End If
        End If

        'If game.keyState.t Then
        If talkFlag Then
            talkFlag = False
            Game.BlueKnight.Moving = False

            REM make monster face the player
            Dim dir As Integer
            dir = Game.combat.GetTargetDirection(npcCenter, Game.hero.CenterPos)
            Game.BlueKnight.Direction = dir
            Game.BlueKnight.AnimationState = Character.AnimationStates.Talking

            REM make player face the NPC
            dir = Game.combat.GetTargetDirection(Game.hero.CenterPos, npcCenter)
            Game.hero.Direction = dir
            Game.hero.AnimationState = Character.AnimationStates.Talking
            Select Case Game.quests.QuestNumber
                Case 4
                    Game.showDialogTutorialKnight1("Sir Neil", "Hello, " + Game.hero.Name + ". I am the gaurd of Kildare gates. To gain entry you must a red key. " _
                                        + "However, I see that you already have one. Very well, you may enter. If you are looking for a place to rest, I recommend " _
                                        + "staying at the inn.")
                Case 5
                    Game.showDialogTutorialKnight1("Sir Neil", "Hello, " + Game.hero.Name + ". I am the gaurd of Kildare gates. To gain entry you must a red key. " _
                    + "However, I see that you already have one. Very well, you may enter. If you are looking for a place to rest, I recommend " _
                    + "staying at the inn.")
                Case Else
                    Game.CasualDialog(Game.BlueKnight)
            End Select
        Else
            Game.BlueKnight.AnimationState = Character.AnimationStates.Standing
        End If
        'End If
    End Sub
#Region "Vendor Methods"
    Public Sub doVendor()
        Dim show As Boolean = False
        Dim relativePos As PointF
        Const talkRadius As Integer = 70
        Dim heroCenter As PointF
        Dim npcCenter As PointF
        Dim dist As Single
        Static target As Integer = 0
        Dim index As Integer = 0

        If Game.inven.Visible Then
            Return
        End If

        Game.Vendor.UpdateTiles()

        If Game.Vendor.X > Game.world.ScrollPos.X - 64 _
        And Game.Vendor.X < Game.world.ScrollPos.X + 23 * 32 + 64 _
        And Game.Vendor.Y > Game.world.ScrollPos.Y - 54 _
        And Game.Vendor.Y < Game.world.ScrollPos.Y + 17 * 32 Then
            heroCenter = Game.hero.CenterPos
            relativePos.X = Game.Vendor.X - Game.world.ScrollPos.X
            relativePos.Y = Game.Vendor.Y - Game.world.ScrollPos.Y
            Game.Vendor.Draw(relativePos)

            REM get center of NPC
            npcCenter = relativePos
            npcCenter.X += Game.Vendor.GetSprite.Width / 2
            npcCenter.Y += Game.Vendor.GetSprite.Height / 2

            REM get distance to the NPC
            dist = Game.hero.CenterDistance(npcCenter)

            REM is player trying to talk to this NPC?
            If dist < talkRadius And Game.Vendor.Alive Then
                Game.Device.DrawEllipse(New Pen(Color.Blue, 2.0), npcCenter.X - 24, npcCenter.Y, 48, 48)
                talkFlag = True
                If Game.casualVisible = False And dialog.Visible = False Then
                    Game.casualVisible = True
                    show = True
                Else
                    show = False
                End If
                Game.hero.AnimationState = Character.AnimationStates.Talking
                Game.Vendor.AnimationState = Character.AnimationStates.Talking
            Else
                Game.casualVisible = False
                talkFlag = False
            End If
        End If

        'If game.keyState.t Then
        If talkFlag Then
            talkFlag = False

            REM make npc face the player
            Dim dir As Integer
            dir = Game.combat.GetTargetDirection(npcCenter, Game.hero.CenterPos)
            Game.Vendor.Direction = dir

            REM make player face the NPC
            dir = Game.combat.GetTargetDirection(Game.hero.CenterPos, npcCenter)
            Game.hero.Direction = dir

            If Game.hero.Level < 3 Then
                Sale1(show)
            ElseIf Game.hero.Level >= 3 And Game.hero.Level < 6 Then
                Sale2(show)
            ElseIf Game.hero.Level >= 6 And Game.hero.Level < 11 Then
                Sale3(show)
            Else

            End If

        Else
            Game.Vendor.AnimationState = Character.AnimationStates.Standing
        End If
        'End If
    End Sub
    Private Sub Sale1(show As Boolean)
        If show = True Then
            showShopDialog("Barholemu the Cheapskate", "Greetings Vistior. My goodness! You look like you've seen a bit of action! I ain't got much, but ye be get'n a fair price! Better " _
               + "'n what ye be get'n over at Nathans. I can tell ye' that much!", 5, "4g rusty dagger", "4g short sword", "4g wooden club", "4g hatchet", "Back" _
               , , , , , , , 4, 4, 4, 4, 0)
        End If

        Select Case dialog.Selection
            Case 1
                If Game.hero.Gold >= 4 Then
                    Game.hero.Gold -= 4
                    Dim sz As SizeF = Game.Device.MeasureString("You bought a rusty dagger for 4 gold.", Game.Font)
                    Game.AddFadingText(Game.hero.GetSprite.X + (Game.hero.GetSprite().Width / 2) - (sz.Width / 2), Game.hero.GetSprite.Y - 25, "You bought a rusty dagger for 4 gold.", Color.Red, 1.0F, New PointF(0, -2.0F))
                    Game.inven.AddItem(Game.items.GetItem("Rusty Dagger"))
                Else
                    Dim sz As SizeF = Game.Device.MeasureString("You don't have enough gold.", Game.Font)
                    Game.AddFadingText(Game.hero.GetSprite.X + (Game.hero.GetSprite().Width / 2) - (sz.Width / 2), Game.hero.GetSprite.Y - 20, "You don't have enough gold.", 1.0F, New PointF(0, -2.0F))
                End If
            Case 2
                If Game.hero.Gold >= 4 Then
                    Game.hero.Gold -= 4
                    Dim sz As SizeF = Game.Device.MeasureString("You bought a short sword for 4 gold.", Game.Font)
                    Game.AddFadingText(Game.hero.GetSprite.X + (Game.hero.GetSprite().Width / 2) - (sz.Width / 2), Game.hero.GetSprite.Y - 20, "You bought a short sword for 4 gold.", 1.0F, New PointF(0, -2.0F))
                    Game.inven.AddItem(Game.items.GetItem("Short Sword"))
                Else
                    Dim sz As SizeF = Game.Device.MeasureString("You don't have enough gold.", Game.Font)
                    Game.AddFadingText(Game.hero.GetSprite.X + (Game.hero.GetSprite().Width / 2) - (sz.Width / 2), Game.hero.GetSprite.Y - 20, "You don't have enough gold.", 1.0F, New PointF(0, -2.0F))
                End If
            Case 3
                If Game.hero.Gold >= 4 Then
                    Game.hero.Gold -= 4
                    Dim sz As SizeF = Game.Device.MeasureString("You bought a woodern club for 4 gold.", Game.Font)
                    Game.AddFadingText(Game.hero.GetSprite.X + (Game.hero.GetSprite().Width / 2) - (sz.Width / 2), Game.hero.GetSprite.Y - 20, "You bought a woodern club for 4 gold.", 1.0F, New PointF(0, -2.0F))
                    Game.inven.AddItem(Game.items.GetItem("Wooden Club"))
                Else
                    Dim sz As SizeF = Game.Device.MeasureString("You don't have enough gold.", Game.Font)
                    Game.AddFadingText(Game.hero.GetSprite.X + (Game.hero.GetSprite().Width / 2) - (sz.Width / 2), Game.hero.GetSprite.Y - 20, "You don't have enough gold.", 1.0F, New PointF(0, -2.0F))
                End If
            Case 4
                If Game.hero.Gold >= 4 Then
                    Game.hero.Gold -= 4
                    Dim sz As SizeF = Game.Device.MeasureString("You bought a hatchet for 4 gold.", Game.Font)
                    Game.AddFadingText(Game.hero.GetSprite.X + (Game.hero.GetSprite().Width / 2) - (sz.Width / 2), Game.hero.GetSprite.Y - 20, "You bought a hatchet for 4 gold.", 1.0F, New PointF(0, -2.0F))
                    Game.inven.AddItem(Game.items.GetItem("Hatchet"))
                Else
                    Dim sz As SizeF = Game.Device.MeasureString("You don't have enough gold.", Game.Font)
                    Game.AddFadingText(Game.hero.GetSprite.X + (Game.hero.GetSprite().Width / 2) - (sz.Width / 2), Game.hero.GetSprite.Y - 20, "You don't have enough gold.", 1.0F, New PointF(0, -2.0F))
                End If
            Case 5
                dialog.Visible = False
        End Select
        If dialog.Selection <> 5 Then
            Game.hero.Frozen = True
            Game.hero._freezeTime = 10
            Game.hero.iceSprite.Image = Game.LoadBitmap("blank square.png")
        Else
            Game.hero.Frozen = False
            Game.hero.iceSprite.Image = Game.LoadBitmap("frozenmagic.png")
        End If
    End Sub
    Private Sub Sale2(show As Boolean)
        If show = True Then
            showShopDialog("Barholemu the Cheapskate", "Greetings Vistior. My goodness! You look like you've seen a bit of action! I ain't got much, but ye be get'n a fair price! Better " _
               + "'n what ye be get'n over at Nathans. I can tell ye' that much!", 5, "16g Axe", "8g Copper Chainmail", "11g Cape of Stamina", "100g Battle Axe", "Back" _
               , , , , , , , 4, 4, 4, 4, 0)
        End If

        Select Case dialog.Selection
            Case 1
                If Game.hero.Gold >= 16 Then
                    Game.hero.Gold -= 16
                    Dim sz As SizeF = Game.Device.MeasureString("You bought a Axe for 16 gold.", Game.Font)
                    Game.AddFadingText(Game.hero.GetSprite.X + (Game.hero.GetSprite().Width / 2) - (sz.Width / 2), Game.hero.GetSprite.Y - 25, "You bought a rusty dagger for 4 gold.", Color.Red, 1.0F, New PointF(0, -2.0F))
                    Game.inven.AddItem(Game.items.GetItem("Axe"))
                Else
                    Dim sz As SizeF = Game.Device.MeasureString("You don't have enough gold.", Game.Font)
                    Game.AddFadingText(Game.hero.GetSprite.X + (Game.hero.GetSprite().Width / 2) - (sz.Width / 2), Game.hero.GetSprite.Y - 20, "You don't have enough gold.", 1.0F, New PointF(0, -2.0F))
                End If
            Case 2
                If Game.hero.Gold >= 8 Then
                    Game.hero.Gold -= 8
                    Dim sz As SizeF = Game.Device.MeasureString("You bought a Copper Chainmail for 8 gold.", Game.Font)
                    Game.AddFadingText(Game.hero.GetSprite.X + (Game.hero.GetSprite().Width / 2) - (sz.Width / 2), Game.hero.GetSprite.Y - 20, "You bought a short sword for 4 gold.", 1.0F, New PointF(0, -2.0F))
                    Game.inven.AddItem(Game.items.GetItem("Copper Chainmail"))
                Else
                    Dim sz As SizeF = Game.Device.MeasureString("You don't have enough gold.", Game.Font)
                    Game.AddFadingText(Game.hero.GetSprite.X + (Game.hero.GetSprite().Width / 2) - (sz.Width / 2), Game.hero.GetSprite.Y - 20, "You don't have enough gold.", 1.0F, New PointF(0, -2.0F))
                End If
            Case 3
                If Game.hero.Gold >= 11 Then
                    Game.hero.Gold -= 11
                    Dim sz As SizeF = Game.Device.MeasureString("You bought a Cape of Stamina for 11 gold.", Game.Font)
                    Game.AddFadingText(Game.hero.GetSprite.X + (Game.hero.GetSprite().Width / 2) - (sz.Width / 2), Game.hero.GetSprite.Y - 20, "You bought a woodern club for 4 gold.", 1.0F, New PointF(0, -2.0F))
                    Game.inven.AddItem(Game.items.GetItem("Cape of Stamina"))
                Else
                    Dim sz As SizeF = Game.Device.MeasureString("You don't have enough gold.", Game.Font)
                    Game.AddFadingText(Game.hero.GetSprite.X + (Game.hero.GetSprite().Width / 2) - (sz.Width / 2), Game.hero.GetSprite.Y - 20, "You don't have enough gold.", 1.0F, New PointF(0, -2.0F))
                End If
            Case 4
                If Game.hero.Gold >= 100 Then
                    Game.hero.Gold -= 100
                    Dim sz As SizeF = Game.Device.MeasureString("You bought a Battle Axe for 4 gold.", Game.Font)
                    Game.AddFadingText(Game.hero.GetSprite.X + (Game.hero.GetSprite().Width / 2) - (sz.Width / 2), Game.hero.GetSprite.Y - 20, "You bought a hatchet for 4 gold.", 1.0F, New PointF(0, -2.0F))
                    Game.inven.AddItem(Game.items.GetItem("Battle Axe"))
                Else
                    Dim sz As SizeF = Game.Device.MeasureString("You don't have enough gold.", Game.Font)
                    Game.AddFadingText(Game.hero.GetSprite.X + (Game.hero.GetSprite().Width / 2) - (sz.Width / 2), Game.hero.GetSprite.Y - 20, "You don't have enough gold.", 1.0F, New PointF(0, -2.0F))
                End If
            Case 5
                dialog.Visible = False
        End Select
        If dialog.Selection <> 5 Then
            Game.hero.Frozen = True
            Game.hero._freezeTime = 10
            Game.hero.iceSprite.Image = Game.LoadBitmap("blank square.png")
        Else
            Game.hero.Frozen = False
            Game.hero.iceSprite.Image = Game.LoadBitmap("frozenmagic.png")
        End If
    End Sub
    Private Sub Sale3(show As Boolean)

    End Sub
#End Region
#End Region
#Region "Shot Methods"
    Public Function getEnemyShot(shooter As Character) As Character
        Dim filename As String = ""
        Select Case shooter.Name
            Case "Red Spider"
                filename = "webFire.char"
            Case "Skeleton Archer"
                filename = "arrowFire.char"
        End Select
        Dim Anatariaile As New Character(game)
        Anatariaile.Load(filename)
        Return Anatariaile
    End Function
    Public Sub FirePlayerShot()
        Dim arrow As New Character(Game)
        Select Case Game.hero.PlayerClass
            Case "Hunter"
                arrow.Load("arrowFire.char")
                PlayBow()
            Case "Priest"
                arrow.Load("earthmagicFire.char")
                PlayFireMagic()
        End Select
        Dim spawnPos As PointF = Game.hero.CenterPos
        spawnPos.X -= arrow.GetSprite.Width / 2
        spawnPos.Y -= arrow.GetSprite.Height / 2
        spawnPos.X += Game.world.ScrollPos.X
        spawnPos.Y += Game.world.ScrollPos.Y
        arrow.Position = spawnPos
        arrow.Direction = Game.hero.Direction
        arrow.Moving = False
        Game.heroProjectiles.Add(arrow)
    End Sub
    Private Sub FireEnemyShot(monster As Character, dir As Integer)
        Dim projectile As Character
        projectile = getEnemyShot(monster)
        Dim spawnPos As PointF = monster.Position
        spawnPos.X += ((monster.GetSprite.Width / 2) - (projectile.GetSprite.Width / 2))
        spawnPos.Y += ((monster.GetSprite.Height / 2) - (projectile.GetSprite.Height / 2))
        projectile.Position = spawnPos
        projectile.Direction = dir
        ' projectile.Velocity = New PointF(projectile.X - game.hero.X, projectile.Y - game.hero.Y)
        projectile.Moving = False
        Game.monsterProjectiles.Add(projectile)
    End Sub
    Private Sub doPlayerShots()
        Dim steps As Integer = CSng(6.0F)
        Dim remove As Boolean = False
        For x As Integer = Game.heroProjectiles.Count - 1 To 0 Step -1
            Select Case Game.heroProjectiles(x).Direction
                Case 1
                    Game.heroProjectiles(x).Y -= steps
                Case 2
                    Game.heroProjectiles(x).X += steps
                    Game.heroProjectiles(x).Y -= steps
                Case 0
                    Game.heroProjectiles(x).X += steps
                Case 5
                    Game.heroProjectiles(x).X += steps
                    Game.heroProjectiles(x).Y += steps
                Case 4
                    Game.heroProjectiles(x).Y += steps
                Case 6
                    Game.heroProjectiles(x).X -= steps
                    Game.heroProjectiles(x).Y += steps
                Case 7
                    Game.heroProjectiles(x).X -= steps
                Case 3
                    Game.heroProjectiles(x).X -= steps
                    Game.heroProjectiles(x).Y -= steps
            End Select

            Try
                animateProjectile(Game.heroProjectiles(x).GetSprite, Game.heroProjectiles(x).Direction, Game.heroProjectiles(x).p_walkColumns, Game.heroProjectiles(x).Position)
            Catch
            End Try

            For Each monster As Character In Game.NPCs
                If Game.Distance(Game.heroProjectiles(x).CenterPos, monster.CenterPos) < 16 Then
                    If monster.Alive Then
                        Game.combat.Target = monster
                        Game.combat.PlayerAttack()
                        If Not Game.heroProjectiles(x).Name = "Arrow" Then
                            PlayMagicHit()
                        End If
                        remove = True
                        Exit For
                    End If
                End If
            Next

            REM monster events
            For Each monster As Character In Game.eventMonsters
                If Game.Distance(Game.heroProjectiles(x).CenterPos, monster.CenterPos) < 16 Then
                    If monster.Alive Then
                        Game.combat.Target = monster
                        Game.combat.PlayerAttack()
                        If Not Game.heroProjectiles(x).Name = "Arrow" Then
                            PlayMagicHit()
                        End If
                        remove = True
                        Exit For
                    End If
                End If
            Next

            If Game.world.GetTile(Game.heroProjectiles(x).GetSprite.CenterPosition.X / 32, Game.heroProjectiles(x).GetSprite.Y / 32).collidable Then
                remove = True
                If Game.heroProjectiles(x).Name = "Arrow" Then
                    PlayArrowHit()
                Else
                    PlayMagicHit()
                End If
            End If
            If Game.heroProjectiles(x).X < 0 Or Game.heroProjectiles(x).X > (32 * 128) Or _
                   Game.heroProjectiles(x).Y < 0 Or Game.heroProjectiles(x).Y > (32 * 128) Then
            End If
            If remove Then
                Game.heroProjectiles.RemoveAt(x)
            End If
        Next
    End Sub
    Private Sub doEnemyShots()
        Dim steps As Integer = CSng(4.2F)
        Dim removed As Boolean = False
        For x As Integer = Game.monsterProjectiles.Count - 1 To 0 Step -1
            Select Case Game.monsterProjectiles(x).Direction
                Case 1
                    Game.monsterProjectiles(x).Y -= steps
                Case 2
                    Game.monsterProjectiles(x).X += steps
                    Game.monsterProjectiles(x).Y -= steps
                Case 0
                    Game.monsterProjectiles(x).X += steps
                Case 5
                    Game.monsterProjectiles(x).X += steps
                    Game.monsterProjectiles(x).Y += steps
                Case 4
                    Game.monsterProjectiles(x).Y += steps
                Case 6
                    Game.monsterProjectiles(x).X -= steps
                    Game.monsterProjectiles(x).Y += steps
                Case 7
                    Game.monsterProjectiles(x).X -= steps
                Case 3
                    Game.monsterProjectiles(x).X -= steps
                    Game.monsterProjectiles(x).Y -= steps
            End Select
            'game.monsterProjectiles(x).X += (game.monsterProjectiles(x).Velocity.X * steps)
            'game.monsterProjectiles(x).Y += (game.monsterProjectiles(x).Velocity.Y * steps)
            Dim relativePos As Point
            relativePos.X = Game.monsterProjectiles(x).X - Game.world.ScrollPos.X
            relativePos.Y = Game.monsterProjectiles(x).Y - Game.world.ScrollPos.Y

            Try
                animateProjectile(Game.monsterProjectiles(x).GetSprite, Game.monsterProjectiles(x).Direction, Game.monsterProjectiles(x).p_walkColumns, Game.monsterProjectiles(x).Position)
            Catch
            End Try

            If Game.monsterProjectiles(x).GetSprite.Bounds.IntersectsWith(New Rectangle(0, 0, 800, 600)) Then
                '   game.monsterProjectiles(x).Draw(relativePos.X, relativePos.Y)
            End If
            Try
                For Each monster As Character In Game.NPCs
                    If Game.Distance(Game.monsterProjectiles(x).CenterPos, New PointF(Game.hero.CenterPos.X + Game.world.ScrollPos.X, Game.hero.CenterPos.Y + Game.world.ScrollPos.Y)) < 16 Then
                        If monster.Alive And Game.hero.Alive Then
                            Game.combat.Target = monster
                            Game.combat.EnemyAttack()
                            If Game.monsterProjectiles(x).Name = "Arrow" Then
                                PlayDamage()
                            ElseIf Game.monsterProjectiles(x).Name = "Earth Magic" Then
                                PlayMagicHit()
                            End If
                            removed = True
                            Exit For
                        End If
                    End If
                Next
                If Game.world.GetTile(Game.monsterProjectiles(x).GetSprite.CenterPosition.X / 32, Game.monsterProjectiles(x).GetSprite.Y / 32).collidable Then
                    removed = True
                    If Game.monsterProjectiles(x).Name = "Arrow" Then
                        PlayArrowHit()
                    ElseIf Game.monsterProjectiles(x).Name = "Earth Magic" Then
                        PlayMagicHit()
                    End If
                End If
                If Not New Rectangle(Game.monsterProjectiles(x).X, Game.monsterProjectiles(x).Y, Game.monsterProjectiles(x).GetSprite.Width, Game.monsterProjectiles(x).GetSprite.Height) _
                    .IntersectsWith(New Rectangle(0, 0, 4096, 4096)) Then
                    removed = True
                End If
                If removed Then
                    Game.monsterProjectiles.RemoveAt(x)
                End If
            Catch ex As Exception
                Console.Write(ex.Message)
            End Try
        Next
    End Sub
    Private Sub animateProjectile(ByRef p_walkSprite As Sprite, p_direction As Integer, p_walkColumns As Integer, p_position As PointF)
        Dim startFrame As Integer
        Dim endFrame As Integer
        p_walkSprite.Position = p_position
        If p_direction > -1 Then
            startFrame = p_direction * p_walkColumns
            endFrame = startFrame + p_walkColumns - 1
            p_walkSprite.AnimationRate = 30
            p_walkSprite.Animate(startFrame, endFrame)
        End If
        If Game.world.IsOnScreen(p_walkSprite.Bounds) Then
            p_walkSprite.Draw(p_position.X - Game.world.ScrollPos.X, p_position.Y - Game.world.ScrollPos.Y)
        End If
    End Sub
#End Region
#Region "Quest Specific Methods"
    Public Sub CheckQuests()
        CheckItemSpawns()
        CheckCharacterSpawns()
        CheckLevels()
        Game.quests.Visible = True
    End Sub
    Public Sub CheckItemSpawns()
        Select Case Game.quests.QuestNumber
            Case 0
                Dim d As New Game.DrawableItem
                d.item = Game.items.GetItem("Gold Key")
                d.sprite = New Sprite(Game)
                d.sprite.Image = Game.LoadBitmap(d.item.DropImageFilename)
                d.sprite.Size = d.sprite.Image.Size
                d.sprite.Position = New Point(384, 160)
                If Not Game.inven.HasItem("Gold Key") AndAlso (Not Game.treasure.Contains(d)) Then
                    Game.LuaDropItem("Gold Key", 384, 160)
                End If
        End Select
    End Sub
    Public Sub CheckCharacterSpawns()
        Select Case Game.quests.QuestNumber
            Case 1
                If Game.world.CurrentLevel = "Training Tutorial.level" Then
                    If Game.tutorialKnight2 Is Nothing Then Game.world.MakeTutorialKnight2()
                End If
            Case 2
                If Game.world.CurrentLevel = "Training Tutorial.level" Then
                    If Game.tutorialKnight2 IsNot Nothing Then
                        Game.tutorialKnight2 = Nothing
                    End If
                    Dim c As Character = New Character(Game)
                    c.Load("red spider.char")
                    c.Position = New Point(123, 186)
                    c.range = New Rectangle(c.Position.X, c.Position.Y, 120, 120)
                    If Not Game.NPCs.Contains(c) Then
                        Game.LuaAddCharacter("red spider.char", 123, 186, 120, 120)
                    End If
                End If
            Case 4
                If Game.world.CurrentLevel = "PillagedVillage.level" Then
                    Game.world.MakePeasant()
                End If
            Case 5
                If Game.world.CurrentLevel = "Forest Entrance.level" Then
                    For n As Integer = 1 To 3 Step 1
                        Dim c As New Character(Game)
                        c.Load("skeleton unarmed.char")
                        c.Position = New PointF(Game.Random(3328, 3328 + 416), Game.Random(192, 192 + 416))
                        c.range = New Rectangle(3328, 192, 416, 416)
                        c.Moving = True
                        Game.NPCs.Add(c)
                    Next
                End If
            Case 6
                If Game.world.CurrentLevel = "Monastary.level" Then
                    Game.world.MakeUrix()
                End If
            Case 10
                If Game.world.CurrentLevel = "Silent Forest(Inner).level" Then
                    Dim c As New Character(Game)
                    c.Load("Green Zombie.char")
                    c.Position = New PointF(2736, 1487)
                    c.range = New Rectangle(c.Position.X, c.Position.Y, 200, 200)
                    If Not Game.NPCs.Contains(c) Then
                        c.Position = New PointF(2736, 1487)
                        c.range = New Rectangle(c.Position.X, c.Position.Y, 200, 200)
                        Game.LuaAddCharacter(c)
                    End If
                End If
            Case 13
                If Game.world.CurrentLevel = "Monastary.level" Then
                    Game.world.MakeUrix()
                End If
        End Select
        REM general level events
        Select Case Game.world.CurrentLevel
            Case "Shop.level"
                Game.world.MakeVendor()
        End Select
        GeneralCharacterSpawns()
    End Sub
    Public Sub GeneralCharacterSpawns()
        Select Case Game.world.CurrentLevel
            Case "East Coast Bottom.level"
                For n As Integer = 0 To 7
                    Game.LuaAddCharacter("red spider.char", 124, 77, 1000, 1000)
                Next
        End Select
    End Sub
    Public Sub CheckLevels()
        Select Case Game.quests.QuestNumber
            Case 3
                If Game.world.CurrentLevel = "Training Tutorial.level" Then
                    Game.world.loadTilemap("Starting Point.level")
                    TeleportPlayer(21, 14)
                End If
        End Select
    End Sub
#End Region
#Region "Commands"
    Public Sub ShowCommandDialog()
        Dim command As String = InputBox("Enter a command.", "Command Terminal")
        Dim cmd() As String = command.Split(" ")
        Try
            If cmd(0) = "Regenerate" Then
                Game.hero.Regenerating = True
                Game.hero.regenPower = Val(cmd(1).ToString)
                Game.hero.drainVictim = Game.NPCs(1)
            ElseIf cmd(0) = "Frozen" Then
                Game.hero.Frozen = True
            ElseIf cmd(0) = "Burn" Then
                Game.hero.Bleeding = Val(cmd(1).ToString)
                Game.hero.Bleeding = True
                Game.hero.bleedPower = 1.0F
            ElseIf cmd(0) = "health" Then
                Game.hero.Health = Val(cmd(1).ToString)
            ElseIf cmd(0) = "xp" Then
                Game.hero.AddCombatXP(Val(cmd(1).ToString))
            ElseIf cmd(0) = "gold" Then
                Game.hero.Gold += Val(cmd(1).ToString)
            ElseIf cmd(0) = "give" Then
                Dim max As Integer = Val(cmd(2).ToString)
                For x As Integer = 1 To max Step 1
                    Game.inven.AddItem(Game.items.GetItem(cmd(1).ToString))
                Next
            ElseIf cmd(0) = "quest" Then
                Game.quests.QuestNumber = Val(cmd(1).ToString)
            ElseIf cmd(0) = "teleport" Then
                Dim dest As Point = New Point(Val(cmd(1)), Val(cmd(2)))
                If cmd(3) <> Game.world.CurrentLevel Then
                    Game.world.loadTilemap(cmd(3))
                End If
                TeleportPlayer(dest.X / 32, dest.Y / 32)
            ElseIf cmd(0) = "spawn" Then
                Dim chara As New Character(Game)
                chara = Game.LuaMakeCharacter(cmd(1), Val(cmd(1)), Val(cmd(2)), Val(cmd(4)), Val(cmd(5)))
                Game.NPCs.Add(chara)
            End If
        Catch
            MsgBox("Invalid Entry", MsgBoxStyle.OkOnly, "Command Terminal")
        End Try
    End Sub
#End Region
#Region "Dialogue Methods"
    Public Sub showShopDialog(ByVal title As String, ByVal message As String, btnNum As Integer, Optional btn1 As String = "", Optional btn2 As String = "", Optional btn3 As String = "", _
    Optional btn4 As String = "", Optional btn5 As String = "", Optional btn6 As String = "", Optional btn7 As String = "", Optional btn8 As String = "", Optional btn9 As String = "", _
    Optional btn10 As String = "", Optional v1 As Integer = 0, Optional v2 As Integer = 0, Optional v3 As Integer = 0, Optional v4 As Integer = 0, Optional v5 As Integer = 0, Optional v6 As Integer = 0, _
    Optional v7 As Integer = 0, Optional v8 As Integer = 0, Optional v9 As Integer = 0, Optional v10 As Integer = 0)
        dialog.Visible = True
        dialog.Selection = 0
        dialog.Title = title
        dialog.Message = message
        dialog.NumButtons = btnNum
        dialog.setButtonText(1, btn1, v1)
        dialog.setButtonText(2, btn2, v2)
        dialog.setButtonText(3, btn3, v3)
        dialog.setButtonText(4, btn4, v4)
        dialog.setButtonText(5, btn5, v5)
        dialog.setButtonText(6, btn6, v6)
        dialog.setButtonText(7, btn7, v7)
        dialog.setButtonText(8, btn8, v8)
        dialog.setButtonText(9, btn9, v9)
        dialog.setButtonText(10, btn10, v10)
    End Sub
    Public Sub showDialogTutorialKnight1(ByVal title As String, ByVal message As String, btnNum As Integer, Optional btn1 As String = "", Optional btn2 As String = "", Optional btn3 As String = "")
        dialog.Title = title
        dialog.Message = message
        dialog.NumButtons = btnNum
        dialog.setButtonText(1, btn1)
        dialog.setButtonText(2, btn2)
        dialog.setButtonText(3, btn3)
        dialog.setCorner(Dialogue.Positions.UpperRight)
        dialog.Visible = True
        dialog.Selection = 1
    End Sub
#End Region

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        End
    End Sub
End Class