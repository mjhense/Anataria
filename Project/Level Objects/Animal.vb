Public Class Animal
    Inherits Character
    Private Const walkTimer As Single = 5000.0F
    Private Const eatTimer As Single = 1000.0F
    Private Const eatChance As Single = 20.0F
    Private walkTime As Single = 0.0F
    Private eatTime As Single = 0
    Private Const standChance As Single = 90.0F
    Private Const standingTime As Single = 1500.0F
    Private standingTimer As Single = 0.0F
    Private character As Character
    Private game As Game
    Public Sub New(ByRef game As Game)
        MyBase.New(game)
        Me.game = game
    End Sub
    Public Sub Update()
        Dim reset As Boolean = False
        If AnimationState = AnimationStates.Walking Then
            walkTime += (1000 / Form1.FrameRate)
        ElseIf AnimationState = AnimationStates.Standing Then
            standingTimer += (1000 / Form1.FrameRate)
        ElseIf AnimationState = AnimationStates.Attacking Then
            eatTime += (1000 / Form1.FrameRate)
        End If
        If walkTime > walkTimer Then
            reset = True
            walkTime = 0.0F
        Else
            Wander()
        End If
        If eatTime > eatTimer Then
            reset = True
            eatTime = 0.0F
        End If
        If standingTimer > standingTime Then
            standingTimer = 0.0F
            reset = True
        End If

        If reset Then
            eatTime = 0.0F
            standingTimer = 0.0F
            walkTime = 0.0F
            Dim chance As Single = game.Random(0, 100)
            If chance <= eatChance Then
                AnimationState = AnimationStates.Talking
            ElseIf chance >= standChance Then
                AnimationState = AnimationStates.Standing
                Moving = False
            Else
                Moving = True
                AnimationState = AnimationStates.Walking
            End If
            PlaySound()
        End If
    End Sub
    Public Sub DrawToScreen()
        Dim relativePos As PointF
        relativePos.X = Position.X - game.world.X
        relativePos.Y = Position.Y - game.world.Y
        If New Rectangle(0, 0, 800, 600).IntersectsWith(New Rectangle(relativePos.X, relativePos.Y, GetSprite.Width, GetSprite.Height)) Then
            Draw(relativePos.X, relativePos.Y)
        End If
    End Sub
    Public Sub PlaySound()
        If game.Random(0, 100) <= 50 Then
            Dim relativePos As PointF
            relativePos.X = Position.X - game.world.X
            relativePos.Y = Position.Y - game.world.Y
            If New Rectangle(0, 0, 800, 600).IntersectsWith(New Rectangle(relativePos.X, relativePos.Y, GetSprite.Width, GetSprite.Height)) Then
                Select Case Name
                    Case "Sheep", "Black Sheep"
                        PlaySheep()
                    Case "Cow"
                        PlayCow()
                    Case "Pig"
                        PlayPig()
                End Select
            End If
        End If
    End Sub
    Public Overrides Sub Wander()
        Dim oldPos As PointF = Position
        UpdateTiles()

        'If p_game.inven.Visible = True Then AnimationState = AnimationStates.Standing

        If Not Alive Then
            Moving = False
            If AnimationState <> AnimationStates.Dying Then AnimationState = AnimationStates.Dead
        End If

        If Frozen Then Return
        If Not Moving Then Return
        Dim steps As Integer = 2

        If AnimationState = AnimationStates.Attacking Or AnimationState = AnimationStates.Dying Or AnimationState = AnimationStates.Talking Or Chase Or game.inven.Visible = True Then
            steps = 0
        End If

        REM move randomly
        Select Case Direction
            Case 1
                Y -= steps
            Case 2
                X += steps * 0.7
                Y -= steps * 0.7
            Case 0
                X += steps
            Case 5
                X += steps * 0.7
                Y += steps * 0.7
            Case 4
                Y += steps
            Case 6
                X -= steps * 0.7
                Y += steps * 0.7
            Case 7
                X -= steps
            Case 3
                X -= steps * 0.7
                Y -= steps * 0.7
        End Select

        REM keep inside boundary
        If X < range.Left Then
            Direction = game.Random(1, 3)
            X = range.Left
        ElseIf X > range.Right - 64 Then
            Direction = game.Random(5, 7)
            X = range.Right - 64
        End If
        If Y < range.Top Then
            Direction = game.Random(3, 5)
            Y = range.Top
        ElseIf Y > range.Bottom - 64 Then
            Direction = game.Random(2) - 1
            If Direction = -1 Then Direction = 7
            Y = range.Bottom - 64
        End If

        REM resolve collidable tile
        Dim pos As Point = GetCurrentTilePos()
        Dim tile As Level.tilemapStruct
        tile = game.world.GetTile(pos.X, pos.Y)
        If (tile.collidable) Then
            Position = oldPos
            Direction = (Direction + 7) Mod 8
            If Direction < 1 Then Direction = 0 - Direction
        ElseIf tile.tempCollidableForMonsters Then
            Position = oldPos
            Direction = (Direction + 7) Mod 8
            If Direction < 1 Then Direction = 0 - Direction
        End If
        REM resolve "walk overs"
        For Each monster As Animal In game.animals
            If monster IsNot Me Then
                If Me.GetCurrentTilePos = monster.GetCurrentTilePos Then
                    Position = oldPos
                    Direction = (Direction + 7) Mod 8
                    If Direction < 1 Then Direction = 0 - Direction
                End If
            End If
        Next
    End Sub
End Class