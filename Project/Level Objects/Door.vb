Public Class Door
    Public Enum AnimationState
        opening
        open
        closed
    End Enum
    Private State As AnimationState
    Private sprite As Sprite
    Public Visible As Boolean
    Private Locked As Boolean
    Private Key As Item
    Private game As Game
    Private type As Integer

    Public ReadOnly Property IsLocked As Boolean
        Get
            Return Locked
        End Get
    End Property

    Public ReadOnly Property CurrentTilePos As Point
        Get
            Return New Point(CInt(Int(sprite.X / 32)), CInt(Int(sprite.Y / 32)))
        End Get
    End Property

    Public Sub New(ByRef game As Game, KeyName As String, type As Integer, x As Single, y As Single)
        Try
            Me.game = game
            Visible = True
            sprite = New Sprite(game)
            sprite.Position = New PointF(x, y)
            sprite.Columns = 3
            Dim size1 As New Size(32, 32)
            Dim size2 As New Size(32, 96)
            Dim size3 As New Size(32, 62.666666666666664)
            Dim size4 As New Size(96, 64)
            Dim size5 As New Size(32, 64)
            Me.type = type
            Select Case type
                Case 1
                    sprite.Image = game.LoadBitmap("door1.png")
                    sprite.TotalFrames = 12
                    sprite.Size = size4
                Case 2
                    sprite.Image = game.LoadBitmap("door2.png")
                    sprite.TotalFrames = 12
                    sprite.Size = size4
                Case 3
                    sprite.Image = game.LoadBitmap("door3.png")
                    sprite.TotalFrames = 9
                    sprite.Size = size1
                Case 4
                    sprite.Image = game.LoadBitmap("door4.png")
                    sprite.TotalFrames = 9
                    sprite.Size = size1
                Case 5
                    sprite.Image = game.LoadBitmap("door5.png")
                    sprite.TotalFrames = 9
                    sprite.Size = size1
                Case 6
                    sprite.Image = game.LoadBitmap("door6.png")
                    sprite.TotalFrames = 9
                    sprite.Size = size5
                Case 7
                    sprite.Image = game.LoadBitmap("door7.png")
                    sprite.TotalFrames = 9
                    sprite.Size = size3
                Case 8
                    sprite.Image = game.LoadBitmap("door8.png")
                    sprite.TotalFrames = 9
                    sprite.Size = size3
                Case 9
                    sprite.Image = game.LoadBitmap("door9.png")
                    sprite.TotalFrames = 9
                    sprite.Size = size3
                Case 10
                    sprite.Image = game.LoadBitmap("door10.png")
                    sprite.TotalFrames = 12
                    sprite.Size = size1
                Case 11
                    sprite.Image = game.LoadBitmap("blank square.png")
                    sprite.TotalFrames = 1
                    sprite.Size = sprite.Image.Size
                Case -1
                    sprite.Image = game.LoadBitmap("blank square.png")
                    sprite.TotalFrames = 1
                    sprite.Size = New Size(32, 32)
            End Select
            State = AnimationState.closed
            If keyName = "" Then
                Locked = False
                Key = Nothing
            ElseIf KeyName = "Event" Or KeyName = "event" Then
                Locked = True
                Key = Nothing
            Else
                Key = game.items.GetItem(KeyName)
                Locked = True
            End If

            'do collidable logic
            For n As Integer = CurrentTilePos.X To CurrentTilePos.X + 3
                For m As Integer = CurrentTilePos.Y To CurrentTilePos.Y + 3
                    If New Rectangle(n * 32, m * 32, 32, 32).IntersectsWith(Me.sprite.Bounds) Then
                        If Locked Then
                            game.world.SetCollidable(New PointF(n, m), True)
                        End If
                    End If
                Next
            Next
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try
    End Sub

    Private Sub Animate()
        If State = AnimationState.closed Then
            sprite.CurrentFrame = 0
        ElseIf State = AnimationState.open Then
            sprite.CurrentFrame = sprite.TotalFrames - 1
        ElseIf State = AnimationState.opening Then
            sprite.Animate(0, sprite.TotalFrames - 1)
        End If
        If sprite.CurrentFrame >= sprite.TotalFrames - 1 Then
            State = AnimationState.open
        End If
    End Sub

    Public Sub ToggleLocked(value As Boolean)
        Locked = value
        If Not Locked Then
            OpenDoor()
        Else
            State = AnimationState.closed
            'do collidable logic
            For n As Integer = CurrentTilePos.X To CurrentTilePos.X + 3
                For m As Integer = CurrentTilePos.Y To CurrentTilePos.Y + 3
                    If New Rectangle(n * 32, m * 32, 32, 32).IntersectsWith(Me.sprite.Bounds) Then
                        If Locked Then
                            game.world.SetCollidable(New PointF(n, m), True)
                        End If
                    End If
                Next
            Next
        End If
    End Sub

    Public Sub Update()
        If Not Visible Then Return

        Animate()
        For x As Integer = CurrentTilePos.X To CurrentTilePos.X + 3
            For y As Integer = CurrentTilePos.Y To CurrentTilePos.Y + 3
                If New Rectangle(x * 32, y * 32, 32, 32).IntersectsWith(Me.sprite.Bounds) Then
                    If Not Locked Then
                        game.world.SetCollidable(New PointF(x, y), False)
                    End If
                End If
            Next
        Next

        If game.doorFlag And game.Distance(game.hero.FootPos, New PointF(sprite.CenterPosition.X - game.world.X, sprite.CenterPosition.Y - game.world.Y)) <= 64 Then
            game.doorFlag = False
            If Locked Then
                If Key IsNot Nothing AndAlso game.inven.HasItem(Key.Name) Then
                    Locked = False
                    game.inven.RemoveOneItem(Key)
                End If
            End If
            If Not Locked Then
                OpenDoor()
            End If
        End If
    End Sub

    Public Sub OpenDoor()
        If Me.State = AnimationState.closed Then
            Me.State = AnimationState.opening
            Select Case type
                Case 0
                    PlayEnterDoor()
                Case 1
                    PlayEnterDoor()
                Case 2
                    PlayEnterDoor()
                Case 3
                    PlayEnterDoor()
                Case 4
                    PlayEnterDoor()
                Case 5
                    PlayMetalDoor()
                Case 6
                    PlayEnterDoor()
                Case 7
                    PlayEnterDoorKnob()
                Case 8
                    PlayEnterDoor()
                Case 9
                    PlayMetalDoor()
                Case 10
                    PlayMetalDoor()
            End Select
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
