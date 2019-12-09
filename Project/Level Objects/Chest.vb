Public Class Chest
    Public Enum AnimationState
        opening
        open
        closed
    End Enum
    Public State As AnimationState
    Public item As Item
    Public sprite As Sprite
    Public gold As Integer
    Public Visible As Boolean
    Private game As Game

    Public ReadOnly Property CurrentTilePos As Point
        Get
            Return New Point(CInt(Int(sprite.X / 32)), CInt(Int(sprite.Y / 32)))
        End Get
    End Property

    Public Sub New(ByRef game As Game, BigChest As Boolean, itemName As String, G As Integer, x As Single, y As Single)
        Try
            Me.game = game
            Visible = True
            sprite = New Sprite(game)
            sprite.Height = 32
            sprite.Width = 32
            sprite.X = x
            sprite.Y = y
            sprite.Columns = 3
            sprite.TotalFrames = 12
            If BigChest Then
                sprite.Image = game.LoadBitmap("Chest_Large.png")
            Else
                sprite.Image = game.LoadBitmap("Chest_Small.png")
            End If
            State = AnimationState.closed
            item = game.items.GetItem(itemName)
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub Animate()
        Select Case State
            Case AnimationState.closed
                sprite.CurrentFrame = 0
            Case AnimationState.open
                sprite.CurrentFrame = 11
            Case AnimationState.opening
                sprite.Animate(0, 11)
                If sprite.CurrentFrame >= 11 Then
                    State = AnimationState.open
                End If
        End Select
    End Sub

    Public Sub Update()
        If Not Visible Then Return

        Animate()

        If game.ChestFlag And game.Distance(game.hero.FootPos, New PointF(sprite.CenterPosition.X - game.world.X, sprite.CenterPosition.Y - game.world.Y)) <= 48 Then
            game.ChestFlag = False
            If Me.State = AnimationState.closed Then
                Me.State = AnimationState.opening
                PlayChest()
                If Me.item IsNot Nothing Then
                    game.inven.AddItem(item)
                End If
                If Me.gold > 0 Then
                    game.hero.Gold += Me.gold
                End If
                Dim str As String
                If item IsNot Nothing Then
                    str = "You got a " + item.Name + " and " + gold.ToString + " gold."
                Else
                    str = "You got " + gold.ToString + " gold."
                End If
                Dim sz As SizeF = game.Device.MeasureString(str, game.Font)
                game.AddFadingText(game.hero.GetSprite.X + (game.hero.GetSprite().Width / 2) - (sz.Width / 2), game.hero.GetSprite.Y - 20, str, 1.0F, New PointF(0, -2.0F))
            End If
        End If
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