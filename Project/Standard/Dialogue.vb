Public Class Dialogue
    Private p_game As Game
    Private p_fontTitle As Font
    Private p_fontMessage As Font
    Private p_fontButton As Font
    Private p_position As PointF
    Private p_corner As Positions
    Private p_size As SizeF
    Private p_title As String
    Private p_message As String
    Private p_buttons(10) As Dialogue.Button
    Private p_numButtons As Integer
    Private p_selection As Integer
    Private p_mousePos As Point
    Private p_mouseBtn As MouseButtons
    Private p_oldMouseBtn As MouseButtons
    Private p_visible As Boolean
    Private p_bg As Bitmap

    Public Enum Positions
        UpperLeft
        LowerLeft
        UpperRight
        LowerRight
    End Enum

    Public Structure Button
        Public Text As String
        Public ShopValue As Integer
        Public sprite As Sprite
    End Structure

    Protected Overrides Sub Finalize()
        p_fontTitle.Dispose()
        p_fontMessage.Dispose()
        p_fontButton.Dispose()
    End Sub

    Public Sub New(ByRef game As Game)
        p_game = game
        p_corner = Positions.UpperRight
        p_size = New SizeF(360, 280)
        p_title = "Title"
        p_message = "Message Text"
        p_fontTitle = New Font("Narkisim", 20, FontStyle.Regular, GraphicsUnit.Pixel)
        p_fontMessage = New Font("Narkisim", 14, FontStyle.Regular, GraphicsUnit.Pixel)
        p_fontButton = New Font("Narkisim", 12, FontStyle.Regular, GraphicsUnit.Pixel)
        p_numButtons = 10
        p_selection = 0
        p_mouseBtn = MouseButtons.None
        p_oldMouseBtn = p_mouseBtn
        p_mousePos = New Point(0, 0)
        p_visible = False
        p_bg = game.LoadBitmap("dialogBG.png")
        Dim p_btnImage As Sprite
        p_btnImage = New Sprite(game)
        p_btnImage.Image = game.LoadBitmap("dialogBtn.png")
        p_btnImage.Size = New Size(200, 50)
        p_btnImage.Position = New PointF(0, 0)
        p_btnImage.CurrentFrame = 0
        p_btnImage.Alive = True
        p_btnImage.AnimationRate = 30.0F
        p_btnImage.Columns = 3
        p_btnImage.TotalFrames = 3
        p_btnImage.CurrentFrame = 0
        For n = 1 To 10
            p_buttons(n).Text = "Button " + n.ToString()
            p_buttons(n).sprite = p_btnImage
        Next
    End Sub

    Public Property Visible() As Boolean
        Get
            Return p_visible
        End Get
        Set(ByVal value As Boolean)
            p_visible = value
        End Set
    End Property

    Public Property Title() As String
        Get
            Return p_title
        End Get
        Set(ByVal value As String)
            p_title = value
        End Set
    End Property

    Public Property Message() As String
        Get
            Return p_message
        End Get
        Set(ByVal value As String)
            p_message = value
        End Set
    End Property

    Public Property NumButtons() As Integer
        Get
            Return p_numButtons
        End Get
        Set(ByVal value As Integer)
            p_numButtons = value
        End Set
    End Property

    Public Sub setButtonText(ByVal index As Integer, ByVal value As String, Optional shopvalue As Integer = 0)
        p_buttons(index).ShopValue = shopvalue
        p_buttons(index).Text = value
    End Sub

    Public Function getButtonText(ByVal index) As String
        Return p_buttons(index).Text
    End Function

    Public Function getButtonRect(ByVal index As Integer) As Rectangle
        Dim i As Integer = index - 1
        Dim rect As New Rectangle(p_position.X, p_position.Y, 0, 0)
        rect.Width = p_size.Width / 2 - 4
        rect.Height = (p_size.Height * 0.4) / 5
        rect.Y += p_size.Height * 0.6 - 4
        Select Case index
            Case 1, 3, 5, 7, 9
                rect.X += 4
                rect.Y += Math.Floor(i / 2) * rect.Height
            Case 2, 4, 6, 8, 10
                rect.X += 4 + rect.Width
                rect.Y += Math.Floor(i / 2) * rect.Height
        End Select
        Return rect
    End Function

    Public Property Selection() As Integer
        Get
            Return p_selection
        End Get
        Set(ByVal value As Integer)
            p_selection = value
        End Set
    End Property

    REM get/set position in pixels 
    Public Property Position() As PointF
        Get
            Return p_position
        End Get
        Set(ByVal value As PointF)
            p_position = value
        End Set
    End Property

    REM set position by corner 
    Public Sub setCorner(ByVal corner As Positions)
        p_corner = corner
    End Sub

    Private Sub Print(ByVal x As Integer, ByVal y As Integer, ByVal text As String, ByVal color As Brush)
        p_game.Device.DrawString(text, p_fontTitle, color, x, y)
    End Sub

    Public Sub updateMouse(ByVal mousePos As Point, ByVal mouseBtn As MouseButtons)
        p_mousePos = mousePos
        p_oldMouseBtn = p_mouseBtn
        p_mouseBtn = mouseBtn
    End Sub

    Public Sub Draw()
        If Not p_visible Then Return

        Select Case p_corner
            Case Positions.UpperLeft
                p_position = New PointF(4, 4)
            Case Positions.LowerLeft
                p_position = New PointF(4, 600 - p_size.Height - 4)
            Case Positions.UpperRight
                p_position = New PointF(800 - p_size.Width - 4, 4)
            Case Positions.LowerRight
                p_position = New PointF(800 - p_size.Width - 4, 600 - p_size.Height - 4)
        End Select

        REM adjust height to fit buttons snugly
        Dim height As Single
        height = 180 + (p_numButtons / 2) * 20

        REM draw background
        p_game.DrawBitmap(p_bg, p_position.X, p_position.Y, p_size.Width, p_size.Height)

        REM draw title
        Dim size As SizeF
        size = p_game.Device.MeasureString(p_title, p_fontTitle)
        Dim tx As Integer = p_position.X + p_size.Width / 2 - size.Width / 2
        Dim ty As Integer = p_position.Y + 6
        p_game.Device.DrawString(p_title, p_fontTitle, Brushes.Orange, tx, ty)

        REM draw message text
        Dim layoutArea As New SizeF(p_size.Width, 120)
        size = p_game.Device.MeasureString(p_message, p_fontMessage, layoutArea, Nothing, p_message.Length(), 4)
        Dim layoutRect As New RectangleF(p_position.X + 8, p_position.Y + 34, size.Width, size.Height)
        p_game.Device.DrawString(p_message, p_fontMessage, Brushes.White, layoutRect)

        REM draw the buttons
        Dim nonSelected As Boolean = False
        For n = 1 To p_numButtons
            Dim rect As Rectangle = getButtonRect(n)

            If rect.Contains(p_mousePos) Then
                REM clicked on this button?
                If p_mouseBtn = MouseButtons.None And p_oldMouseBtn = MouseButtons.Left Then
                    p_selection = n
                    nonSelected = True
                End If
                If Not p_buttons(n).sprite.CurrentFrame = 2 Then
                    p_buttons(n).sprite.Animate()
                End If
            Else
                p_buttons(n).sprite.CurrentFrame = 0
            End If
            If Not nonSelected Then
                p_selection = 0
            End If
            p_buttons(n).sprite.Draw(rect)


            REM print button label 
            size = p_game.Device.MeasureString(p_buttons(n).Text, p_fontButton)
            tx = rect.X + rect.Width / 2 - size.Width / 2
            ty = rect.Y + 2
            p_game.Device.DrawString(p_buttons(n).Text, p_fontButton, Brushes.White, tx, ty)

        Next

    End Sub

End Class

