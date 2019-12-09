Imports Microsoft.Xna.Framework.Input
Public Class Title
    Public Enum TitleStates
        NONE
        CONTINUE_GAME
        NEW_GAME
        EXIT_GAME
    End Enum
    Public Structure MainButton
        Public sprite As Sprite
    End Structure
    Public Structure KeyStates
        Public up, down, right, left, enter As Boolean
    End Structure
    Private Enum ButtonState
        Released
        Pressed
    End Enum
    Private pos As PointF
    Private btnUp As ButtonState
    Private btnDown As ButtonState
    Private oldDown As ButtonState
    Private oldbtnUp As ButtonState
    Private btn As ButtonState
    Private oldbtn As ButtonState
    Private titleState As TitleStates
    Private NewGame As MainButton
    Private ContinueGame As MainButton
    Private ExitGame As MainButton
    Private game As Game
    Private font As Font
    Private bg As Bitmap
    Private btnSize As Rectangle = New Rectangle(0, 0, 396, 35)
    Private oldState As TitleStates
    Public keyState As KeyStates
    Public Sub New(game As Game)
        PlayTheme()
        Me.game = game
        font = New Font("Narkisim", 12)
        NewGame = New MainButton
        ContinueGame = New MainButton
        ExitGame = New MainButton
        LoadSprite(NewGame.sprite)
        LoadSprite(ExitGame.sprite)
        LoadSprite(ContinueGame.sprite)
        bg = game.LoadBitmap("TitleScreenOverlay.png")
        btn = ButtonState.Released
        titleState = TitleStates.NONE
        Dim x As Integer = (Form1.Width / 2) - (396 / 2)
        NewGame.sprite.Position = New PointF(x, 296)
        ContinueGame.sprite.Position = New PointF(x, 296 + 38)
        ExitGame.sprite.Position = New PointF(x, 296 + 38 + 38)
    End Sub
    Private Sub LoadSprite(ByRef sprite As Sprite)
        sprite = New Sprite(game)
        sprite.Size = New Size(btnSize.Width, btnSize.Height)
        sprite.Image = game.LoadBitmap("ButtonAnim.png")
        sprite.Columns = 3
        sprite.TotalFrames = 3
        sprite.AnimationRate = 16
    End Sub
    Private Sub updateButtons()
        ' If pad.IsConnected Then
        ' oldbtn = btn
        ' oldbtnUp = btnUp
        ' oldDown = btnDown
        ' btn = pad.Buttons.A
        ' btnDown = pad.DPad.Down
        ' btnUp = pad.DPad.Up
        ' Else
        oldbtn = btn
        oldbtnUp = btnUp
        oldDown = btnDown
        '  Dim keyState As KeyboardState = Keyboard.GetState()
        If keyState.up Then
            btnUp = ButtonState.Pressed
        Else
            btnUp = ButtonState.Released
        End If
        If keyState.down Then
            btnDown = ButtonState.Pressed
        Else
            btnDown = ButtonState.Released
        End If
        If keyState.enter Then
            btn = ButtonState.Pressed
        Else
            btn = ButtonState.Released
        End If
        'End If
    End Sub
    Private Sub Update()
        'Dim padState As GamePadState = GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One)
        updateButtons()
        oldState = titleState

        'figure button presses
        If btnDown = ButtonState.Released And oldDown = ButtonState.Pressed Then
            Select Case titleState
                Case TitleStates.NONE
                    titleState = TitleStates.NEW_GAME
                Case TitleStates.NEW_GAME
                    titleState = TitleStates.CONTINUE_GAME
                Case TitleStates.CONTINUE_GAME
                    titleState = TitleStates.EXIT_GAME
                Case TitleStates.EXIT_GAME
                    titleState = TitleStates.NEW_GAME
            End Select
        End If
        If btnUp = ButtonState.Released And oldbtnUp = ButtonState.Pressed Then
            Select Case titleState
                Case TitleStates.NONE
                    titleState = TitleStates.NEW_GAME
                Case TitleStates.EXIT_GAME
                    titleState = TitleStates.CONTINUE_GAME
                Case TitleStates.NEW_GAME
                    titleState = TitleStates.EXIT_GAME
                Case TitleStates.CONTINUE_GAME
                    titleState = TitleStates.NEW_GAME
            End Select
        End If

        'check for animations
        SetAllAnimateToFalse()
        Select Case titleState
            Case TitleStates.CONTINUE_GAME
                NewGame.sprite.CurrentFrame = 0
                ExitGame.sprite.CurrentFrame = 0
                ContinueGame.sprite.Alive = True
            Case TitleStates.NEW_GAME
                ExitGame.sprite.CurrentFrame = 0
                ContinueGame.sprite.CurrentFrame = 0
                NewGame.sprite.Alive = True
            Case TitleStates.EXIT_GAME
                ContinueGame.sprite.CurrentFrame = 0
                NewGame.sprite.CurrentFrame = 0
                ExitGame.sprite.Alive = True
        End Select

        'check for button press
        If btn = ButtonState.Released And oldbtn = ButtonState.Pressed Then
            PlayDecision3()
            Select Case titleState
                Case TitleStates.EXIT_GAME
                    If ExitGame.sprite.CurrentFrame = 2 Then
                        StopTheme()
                        End
                    End If
                Case TitleStates.NEW_GAME
                    game.gameState = Anataria.Game.GameStates.STATE_CHARACTER
                Case TitleStates.CONTINUE_GAME
                    game.Device.Clear(Color.Black)
                    game.inven.Visible = False
                    PlayLoad()
                    StopTheme()
                    game.hero.LoadGame(game.savegamefile)
                    Form1.RespawnHero()
                    game.gameState = Anataria.Game.GameStates.STATE_PLAYING
            End Select
        End If

        If Not NewGame.sprite.CurrentFrame = 2 Then
            NewGame.sprite.Animate()
        End If
        If Not ContinueGame.sprite.CurrentFrame = 2 Then
            ContinueGame.sprite.Animate()
        End If
        If Not ExitGame.sprite.CurrentFrame = 2 Then
            ExitGame.sprite.Animate()
        End If

        If oldState <> titleState Then
            PlayCursor2()
        End If
    End Sub
    Private Sub Draw()
        'draw the background
        game.Device.DrawImage(bg, New Rectangle(0, 0, Form1.Width, Form1.Height))
        'draw the button sprites
        NewGame.sprite.Draw()
        ContinueGame.sprite.Draw()
        ExitGame.sprite.Draw()
        'print text
        Dim str As String = "New Game"
        Dim sz As PointF = game.Device.MeasureString(str, font)
        Dim rect As PointF
        rect = New PointF(CSng(NewGame.sprite.X + ((NewGame.sprite.Width / 2) - (sz.X / 2))), CSng(NewGame.sprite.Y + ((NewGame.sprite.Height / 2) - (sz.Y / 2))))
        game.Device.DrawString(str, font, Brushes.Orange, rect)
        str = "Continue Game"
        sz = game.Device.MeasureString(str, font)
        rect = New PointF(CSng(ContinueGame.sprite.X + ((ContinueGame.sprite.Width / 2) - (sz.X / 2))), CSng(ContinueGame.sprite.Y + ((ContinueGame.sprite.Height / 2) - (sz.Y / 2))))
        game.Device.DrawString(str, font, Brushes.Orange, rect)
        str = "Exit Game"
        sz = game.Device.MeasureString(str, font)
        rect = New PointF(CSng(ExitGame.sprite.X + ((ExitGame.sprite.Width / 2) - (sz.X / 2))), CSng(ExitGame.sprite.Y + ((ExitGame.sprite.Height / 2) - (sz.Y / 2))))
        game.Device.DrawString(str, font, Brushes.Orange, rect)
    End Sub
    Public Sub doTitle()
        Update()
        Draw()
    End Sub
    Private Sub SetAllAnimateToFalse()
        Select Case titleState
            Case TitleStates.CONTINUE_GAME
                NewGame.sprite.Alive = False
                NewGame.sprite.CurrentFrame = 0
                ExitGame.sprite.Alive = False
                ExitGame.sprite.CurrentFrame = 0
            Case TitleStates.EXIT_GAME
                NewGame.sprite.Alive = False
                NewGame.sprite.CurrentFrame = 0
                ContinueGame.sprite.Alive = False
                ContinueGame.sprite.CurrentFrame = 0
            Case TitleStates.NEW_GAME
                ContinueGame.sprite.Alive = False
                ContinueGame.sprite.CurrentFrame = 0
                ExitGame.sprite.Alive = False
                ExitGame.sprite.CurrentFrame = 0
            Case TitleStates.NONE
                NewGame.sprite.Alive = False
                NewGame.sprite.CurrentFrame = 0
                ContinueGame.sprite.Alive = False
                ContinueGame.sprite.CurrentFrame = 0
                ExitGame.sprite.Alive = False
                ExitGame.sprite.CurrentFrame = 0
        End Select
    End Sub
End Class