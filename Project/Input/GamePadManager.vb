Imports Microsoft.Xna.Framework.Input
Public Module GamePadManager
    Private game As Game
    Private oldX As ButtonState
    Private oldY As ButtonState
    Private oldA As ButtonState
    Private oldB As ButtonState
    Private oldStart As ButtonState
    Private oldSelect As ButtonState
    Private oldR As ButtonState
    Private oldL As ButtonState
    Private oldBigBtn As ButtonState
    Private X As ButtonState
    Private Y As ButtonState
    Private A As ButtonState
    Private B As ButtonState
    Private Start As ButtonState
    Private SelectBtn As ButtonState
    Private R As ButtonState
    Private L As ButtonState
    Private BigBtn As ButtonState
    Public Sub Initialize(ByRef game1 As Game)
        game = game1
    End Sub
    Private Sub UpdateButtonPresses(padState As GamePadState)
        oldX = X
        oldY = Y
        oldA = A
        oldB = B
        oldL = L
        oldR = R
        oldStart = Start
        oldSelect = SelectBtn
        oldBigBtn = BigBtn
        X = padState.Buttons.X
        Y = padState.Buttons.Y
        A = padState.Buttons.A
        B = padState.Buttons.B
        Start = padState.Buttons.Start
        SelectBtn = padState.Buttons.Back
        L = padState.Buttons.LeftShoulder
        R = padState.Buttons.RightShoulder
        BigBtn = padState.Buttons.BigButton
    End Sub
    Private Sub HandleMouseMovement(padState As GamePadState)
        Dim steps As Single = 12.0F
        Dim mouseState As MouseState = Mouse.GetState
        Dim stick As Microsoft.Xna.Framework.Vector2 = padState.ThumbSticks.Right
        stick.X *= steps
        stick.Y *= -1
        stick.Y *= steps
        Mouse.SetPosition(mouseState.X + stick.X, mouseState.Y + stick.Y)
        If A = ButtonState.Pressed Then
            game.MouseButton = MouseButtons.Left
        Else
            game.MouseButton = MouseButtons.None
        End If
    End Sub
    Private Sub HandleThumbSticks(padState As GamePadState)
        Dim stick As Microsoft.Xna.Framework.Vector2 = padState.ThumbSticks.Left
        If stick.X < -0.3F Then
            game.keyState.left = True
        Else
            game.keyState.left = False
        End If
        If stick.X >= 0.3F Then
            game.keyState.right = True
        Else
            game.keyState.right = False
        End If
        If stick.Y < -0.3F Then
            game.keyState.down = True
        Else
            game.keyState.down = False
        End If
        If stick.Y > 0.3F Then
            game.keyState.up = True
        Else
            game.keyState.up = False
        End If
    End Sub
    Public Sub UpdatePad()
        Dim padState As GamePadState = GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One)
        If padState.IsConnected = False Then Return
        UpdateButtonPresses(padState)
        HandleMouseMovement(padState)
        'handle gamestate specific button presses
        Select Case game.gameState
            Case Project.Game.GameStates.STATE_PAUSED
                If Start = ButtonState.Released And oldStart <> Start Then
                    game.gameState = Project.Game.GameStates.STATE_PLAYING
                End If
            Case Project.Game.GameStates.STATE_PLAYING
                HandleThumbSticks(padState)
                If Start = ButtonState.Released And oldStart = ButtonState.Pressed Then game.gameState = Project.Game.GameStates.STATE_PAUSED
                If L = ButtonState.Released And oldL <> L Then
                    game.ChestFlag = True
                    game.doorFlag = True
                    game.switchFlag = True
                    Form1.lootFlag = True
                End If
                If R = ButtonState.Released And oldR <> R Then game.keyState.shift = True
                If X = ButtonState.Released And oldX <> X Then game.gameState = Project.Game.GameStates.STATE_STATS
                If Y = ButtonState.Released And oldY <> Y Then game.quests.Visible = Not game.quests.Visible
                If B = ButtonState.Released And B <> oldB Then game.inven.Visible = Not game.inven.Visible
            Case Project.Game.GameStates.STATE_STATS
                game.keyState.right = False
                game.keyState.left = False
                game.keyState.shift = False
                game.keyState.up = False
                game.keyState.down = False
                If X = ButtonState.Released And oldX = ButtonState.Pressed Then
                    game.gameState = Project.Game.GameStates.STATE_PLAYING
                End If
        End Select
        REM button handling for all states 
        If SelectBtn = ButtonState.Released And oldSelect <> SelectBtn Then
            REM handle level specific teleports
            Select Case game.world.CurrentLevel
                Case "Silent Temple F1.level"
                    Form1.TeleportPlayer(62, 126)
                Case "Silent Temple F2.level"
                    Form1.TeleportPlayer(64, 14)
            End Select
            game.hero.SaveGame(game.savegamefile)
            game.gameState = Project.Game.GameStates.STATE_TITLE
        End If
    End Sub
End Module