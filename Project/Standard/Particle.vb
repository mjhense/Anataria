Public Class Particle
    Inherits Sprite
    Private game As Game
    Public Property Expired As Boolean
    Public Property Rect As Rectangle
    Public Sub New(ByRef game As Game, rect As Rectangle)
        MyBase.New(game)
        Me.game = game
        Expired = False
        Me.Rect = rect
    End Sub
    Public Sub New(ByRef game As Game)
        MyBase.New(game)
        Me.game = game
        Expired = False
    End Sub
    Public Overrides Sub Animate(startFrame As Integer, endFrame As Integer)
        If TotalFrames > 0 Then
            Dim time As Integer = Environment.TickCount()
            If time > p_lastTime + AnimationRate Then
                p_lastTime = time
                CurrentFrame += 1
                If CurrentFrame > endFrame Then
                    CurrentFrame = 0
                    Expired = True
                End If
            End If
        End If
    End Sub
    Public Overrides Sub Draw()
        If Expired Then Return
        MyBase.Draw(rect)
    End Sub
End Class