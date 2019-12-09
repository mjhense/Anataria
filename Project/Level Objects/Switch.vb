Public Class Switch
    Public Enum AnimationState
        opening
        open
        closed
    End Enum
    Private State As AnimationState
    Private sprite As Sprite
    Private Visible As Boolean
    Private game As Game
    Private Flipped As Boolean
    Private type As Integer

    Public Sub ToggleFilipped(flipped As Boolean)
        Me.Flipped = flipped
        If Not flipped Then
            State = AnimationState.closed
        End If
    End Sub

    Public ReadOnly Property CurrentTilePos As Point
        Get
            Return New Point(CInt(Int(sprite.X / 32)), CInt(Int(sprite.Y / 32)))
        End Get
    End Property

    Public ReadOnly Property IsFlipped As Boolean
        Get
            Return Flipped
        End Get
    End Property

    Public Sub New(ByRef game As Game, type As Integer, x As Single, y As Single)
        Try
            Me.type = type
            Me.game = game
            Visible = True
            sprite = New Sprite(game)
            sprite.Height = 32
            sprite.Width = 32
            sprite.X = x
            sprite.Y = y
            sprite.Image = game.LoadBitmap("switch " + type.ToString + ".png")
            sprite.AnimationRate = 10
            Select Case type
                Case 0
                    sprite.Height = 32
                    sprite.Width = 32
                    sprite.Columns = 4
                    sprite.TotalFrames = 4
                Case 1
                    sprite.Height = 64
                    sprite.Width = 64
                    sprite.Columns = 8
                    sprite.TotalFrames = 8
            End Select
            State = AnimationState.closed
            Me.Flipped = False
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub Animate()
        Select Case State
            Case AnimationState.closed
                sprite.CurrentFrame = 0
                Me.Flipped = False
            Case AnimationState.open
                sprite.CurrentFrame = sprite.TotalFrames - 1
            Case AnimationState.opening
                sprite.Animate()
                If sprite.CurrentFrame >= sprite.TotalFrames - 1 Then
                    State = AnimationState.open
                    Me.Flipped = True
                End If
        End Select
    End Sub

    Public Sub Update()
        If Not Visible Then Return

        Animate()

        Select Case type
            Case 0
                If CurrentTilePos = game.hero.GetCurrentTilePos Then
                    If Me.State = AnimationState.closed Then
                        Me.State = AnimationState.opening
                        PlaySwitch()
                    End If
                End If
            Case 1
                If game.switchFlag And game.Distance(game.hero.FootPos, New PointF(sprite.CenterPosition.X - game.world.X, sprite.CenterPosition.Y - game.world.Y)) <= 70 Then
                    game.switchFlag = False
                    If Me.State = AnimationState.closed Then
                        Me.State = AnimationState.opening
                        PlaySwitch()
                    End If
                End If
        End Select
    End Sub

    Public Sub DrawToScreen()
        If Not Visible Then Return
        If New Rectangle(game.world.ScrollPos.X, game.world.ScrollPos.Y, 800, 600).IntersectsWith(sprite.Bounds) Then
            Dim relativePos As Point
            relativePos.X = sprite.X - game.world.X
            relativePos.Y = sprite.Y - game.world.Y
            sprite.Draw(relativePos.X, relativePos.Y)
        End If
    End Sub
End Class
