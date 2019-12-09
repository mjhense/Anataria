Public Class AnimationStrip
    Public Property Position As PointF
    Public Property Image As Bitmap
    Public Property Size As Size
    Public Frames As New List(Of Rectangle)
    Public CurrentFrame As Integer
    Public Property Visible As Boolean
    Private game As Game

    Public ReadOnly Property Bounds As Rectangle
        Get
            Return New Rectangle(Position.X, Position.Y, Size.Width, Size.Height)
        End Get
    End Property

    Public Sub New(ByRef game As Game, initialFrame As Rectangle, image As Bitmap, frames As Integer)
        Me.game = game
        Me.Image = image
        Me.Visible = False
        Me.Size = New Size(initialFrame.Width, initialFrame.Height)
        Me.Visible = True
        Me.CurrentFrame = 0
        For n As Integer = 0 To frames - 1
            Me.Frames.Add(New Rectangle(initialFrame.X * n, initialFrame.Y, initialFrame.Width, initialFrame.Height))
        Next
    End Sub

    Public Sub Draw()
        If Not Visible Then Return
        CurrentFrame += 1
        If CurrentFrame > Frames.Count - 1 Then
            CurrentFrame = 0
        End If

        game.Device.DrawImage(Image, New Rectangle(Position.X, Position.Y, Size.Width, Size.Height), Frames(CurrentFrame), GraphicsUnit.Pixel)
    End Sub

    Public Function IsColliding(other As Sprite) As Boolean
        Return IsColliding(other.Bounds)
    End Function

    Public Function IsColliding(other As Rectangle) As Boolean
        If Me.Bounds.IntersectsWith(other) Then
            Return True
        Else
            Return False
        End If
    End Function
End Class