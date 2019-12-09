Public Class LaserCannon
    Public Enum Colors
        green
        black
        blue
        red
        yellow
    End Enum
    Public Structure Laser
        Public sprite As Sprite
        Public velocity As PointF
    End Structure
    Private sprite As Sprite
    Private color As Colors
    Private rotationTimer As Single
    Private Const rotationTotalTime As Single = 4000.0F
    Private shotTimer As Single
    Private Const shotDelay As Single = 32.0F
    Private lasers As New List(Of Laser)
    Private game As Game
    Public Property Position As PointF
        Get
            Return sprite.Position
        End Get
        Set(value As PointF)
            sprite.Position = value
        End Set
    End Property
    Public Property X As Single
        Get
            Return sprite.X
        End Get
        Set(value As Single)
            sprite.X = value
        End Set
    End Property
    Public Property Y As Single
        Get
            Return sprite.Y
        End Get
        Set(value As Single)
            sprite.Y = value
        End Set
    End Property
    Public ReadOnly Property CenterPos As PointF
        Get
            Return sprite.CenterPosition
        End Get
    End Property
    Public ReadOnly Property Bounds As Rectangle
        Get
            Return sprite.Bounds
        End Get
    End Property
    Public ReadOnly Property CanFire As Boolean
        Get
            Return shotTimer = 0
        End Get
    End Property
    Public Sub New(ByRef game As Game, color As Colors, tileX As Integer, tileY As Integer)
        Me.game = game
        sprite = New Sprite(game)
        sprite.Size = New Size(64, 80)
        lasers.Clear()
        sprite.TotalFrames = 32
        sprite.Columns = 16
        sprite.AnimationRate = 8
        Me.color = color
        Me.Position = New PointF(tileX * 32, tileY * 32)
        Select Case color
            Case Colors.black : sprite.Image = game.LoadBitmap("black laser cannon.png")
            Case Colors.blue : sprite.Image = game.LoadBitmap("blue laser cannon.png")
            Case Colors.green : sprite.Image = game.LoadBitmap("green laser cannon.png")
            Case Colors.red : sprite.Image = game.LoadBitmap("red laser cannon.png")
            Case Colors.yellow : sprite.Image = game.LoadBitmap("yellow laser cannon.png")
        End Select
        shotTimer = 0.0F
        rotationTimer = 0.0F
        For n As Integer = tileX To tileX + 1
            For m As Integer = tileY To tileY + 2
                game.world.SetTempCollidable(New PointF(n, m), True)
            Next
        Next
    End Sub
    Private Sub FireShot()
        ' If Not CanFire Then Return
        Dim laser As New Laser
        Dim shot As New Sprite(game)
        Dim shotOffset As PointF
        Dim ox As Single = 25.0F
        Dim oy As Single = 25.0F
        shot.Size = New Size(4, 4)
        shot.Columns = 1
        shot.TotalFrames = 1
        Select Case color
            Case Colors.black : shot.Image = game.LoadBitmap("black laser.png")
            Case Colors.blue : shot.Image = game.LoadBitmap("blue laser.png")
            Case Colors.green : shot.Image = game.LoadBitmap("green laser.png")
            Case Colors.red : shot.Image = game.LoadBitmap("red laser.png")
            Case Colors.yellow : shot.Image = game.LoadBitmap("yellow laser.png")
        End Select
        shot.CurrentFrame = (rotationTimer / rotationTotalTime) * 16
        Select Case sprite.CurrentFrame
            Case 0, 1
                laser.velocity = New PointF(0, 2)
                shotOffset = New PointF(0, 25)
            Case 2, 3
                laser.velocity = New PointF(-1, 2)
                shotOffset = New PointF(-25, 25)
            Case 4, 5, 6
                laser.velocity = New PointF(-2, 2)
                shotOffset = New PointF(-25, 25)
            Case 7, 8, 9
                laser.velocity = New PointF(-2, 0)
                shotOffset = New PointF(-25, 0)
            Case 10, 11, 12
                laser.velocity = New PointF(-2, -2)
                shotOffset = New PointF(-25, -25)
            Case 13, 14
                laser.velocity = New PointF(-2, -2)
                shotOffset = New PointF(-25, -25)
            Case 15, 16
                laser.velocity = New PointF(0, -2)
                shotOffset = New PointF(0, -25)
            Case 17, 18
                laser.velocity = New PointF(1, -2)
                shotOffset = New PointF(25, -25)
            Case 19, 20, 21
                laser.velocity = New Point(2, -2)
                shotOffset = New PointF(25, -25)
            Case 22, 23, 24
                laser.velocity = New PointF(2, 0)
                shotOffset = New PointF(25, 0)
            Case 25, 26, 27
                laser.velocity = New PointF(2, 2)
                shotOffset = New PointF(25, 25)
            Case 28, 29, 30
                laser.velocity = New PointF(1, 2)
                shotOffset = New PointF(25, -25)
            Case 30, 31
                laser.velocity = New PointF(0, 2)
                shotOffset = New PointF(0, 25)
        End Select
        shot.Position = sprite.CenterPosition
        shot.X += shotOffset.X
        shot.Y += shotOffset.Y
        laser.sprite = shot
        lasers.Add(laser)
    End Sub
    Public Sub update()
        rotationTimer += 1000 / game.FrameRate
        If rotationTimer >= rotationTotalTime Then rotationTimer = 0.0F
        sprite.Animate()

        shotTimer += 16
        If shotTimer > shotDelay Then
            shotTimer = 0.0F
            FireShot()
        End If

        REM update shots
        For n As Integer = lasers.Count - 1 To 0 Step -1
            Dim expired As Boolean = False
            lasers(n).sprite.X += lasers(n).velocity.X
            lasers(n).sprite.Y += lasers(n).velocity.Y
            If game.world.GetTile((lasers(n).sprite.X / 32), (lasers(n).sprite.Y / 32)).collidable Then
                expired = True
            End If
            If New Rectangle(lasers(n).sprite.X, lasers(n).sprite.Y, lasers(n).sprite.Width, lasers(n).sprite.Height).IntersectsWith(game.hero.GetSprite.Bounds) Then
                expired = True
                game.hero.Health -= 5.0F
            End If
            If expired Then lasers.RemoveAt(n)
        Next
    End Sub
    Public Sub Draw()
        Dim relativePos As PointF
        relativePos.X = sprite.X - game.world.ScrollPos.X
        relativePos.Y = sprite.Y - game.world.ScrollPos.Y
        If New Rectangle(relativePos.X, relativePos.Y, sprite.Width, sprite.Height).IntersectsWith(New Rectangle(0, 0, 800, 600)) Then
            sprite.Draw(relativePos.X, relativePos.Y)
        End If
        For n As Integer = lasers.Count - 1 To 0 Step -1
            relativePos.X = lasers(n).sprite.X - game.world.ScrollPos.X
            relativePos.Y = lasers(n).sprite.Y - game.world.ScrollPos.Y
            If New Rectangle(relativePos.X, relativePos.Y, lasers(n).sprite.Width, lasers(n).sprite.Height).IntersectsWith(New Rectangle(0, 0, 800, 600)) Then
                lasers(n).sprite.Draw(relativePos.X, relativePos.Y)
            Else
                lasers.RemoveAt(n)
            End If
        Next
    End Sub
End Class