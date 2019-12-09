Public Class NPC
    Inherits Character
    Private game As Game
    Private _talkFlag As Boolean
    Public talkRadius As Single = 70.0F
    Public Property TalkFlag As Boolean
        Get
            Return _talkFlag
        End Get
        Set(value As Boolean)
            _talkFlag = value
        End Set
    End Property
    Public Sub New(ByRef game As Game)
        MyBase.New(game)
        Me.game = game
        _talkFlag = False
    End Sub
    Public Overridable Sub Update()
        If game.Distance(game.hero.WorldPosition, Me.Position) <= talkRadius Then TalkFlag = True
        If TalkFlag Then Moving = False
        If Moving Then Wander()
        Me.UpdateTiles()
    End Sub
    Public Overridable Sub Update(ByRef dialog As Dialogue)
        If game.Distance(game.hero.WorldPosition, Me.Position) <= talkRadius Then TalkFlag = True
        If TalkFlag Then Moving = False
        If Moving Then Wander()
        Me.UpdateTiles()
    End Sub
    Public Overrides Sub UpdateTiles()
        oldTilePos = newTilePos
        newTilePos = GetCurrentTilePos()
        For n As Integer = newTilePos.X To newTilePos.X + 1 Step 1
            For m As Integer = newTilePos.Y To newTilePos.Y + 1 Step 1
                Dim p As New PointF(n, m)
                If Me.Alive Then
                    If New Rectangle(n * 32, m * 32, 32, 32).IntersectsWith(New Rectangle(X, Y, GetSprite.Width, GetSprite.Height)) Then
                        game.world.SetTempCollidable(p, True)
                        game.world.SetTempCollidableForMonsters(p, True)
                    End If
                Else
                    If New Rectangle(n * 32, m * 32, 32, 32).IntersectsWith(New Rectangle(X, Y, GetSprite.Width, GetSprite.Height)) Then
                        game.world.SetTempCollidable(p, False)
                        game.world.SetTempCollidableForMonsters(p, False)
                    End If
                End If
            Next
        Next
    End Sub
    Public Sub DrawToScreen()
        Dim relativePos As New PointF(Me.X - game.world.X, Me.Y - game.world.Y)
        If New Rectangle(relativePos.X, relativePos.Y, GetSprite.Width, GetSprite.Height).IntersectsWith(New Rectangle(0, 0, 800, 600)) Then
            Me.Draw(relativePos.X, relativePos.Y)
        End If
    End Sub
End Class