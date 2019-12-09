Public Class Inventory

    Public Structure Button
        Public rect As Rectangle
        Public text As String
        Public bgImage As Bitmap
        Public image As Bitmap
        Public imagefile As String
    End Structure

    Public BTN_HEAD As Integer
    Public BTN_CHEST As Integer
    Public BTN_LEGS As Integer
    Public BTN_RTHAND As Integer
    Public BTN_LTHAND As Integer
    Public BTN_RTFINGER As Integer
    Public BTN_LTFINGER As Integer
    Public BTN_DESTROY As Integer
    Public BTN_USE As Integer

    Private p_game As Game
    Private p_font As Font
    Private p_font2 As Font
    Private p_position As PointF
    Private p_buttons(32) As Button
    Private p_selection As Integer
    Private p_sourceIndex As Integer
    Private p_targetIndex As Integer
    Private p_mousePos As Point
    Private p_mouseBtn As MouseButtons
    Private p_lastButton As Integer
    Private p_oldMouseBtn As MouseButtons
    Private p_visible As Boolean
    Private p_bg As Bitmap
    Private p_btnHighlight As Bitmap
    Private p_inventory(32) As Item
    Public greenCrystal As Sprite
    Public redCrystal As Sprite
    Public blueCrystal As Sprite
    Public yellowCrystal As Sprite
    Public purpleCrystal As Sprite
    Private scrollBtn As Sprite
    Private scrollPos As PointF

    Public Sub New(ByRef game As Game, ByVal pos As Point)
        p_game = game
        p_position = pos
        p_bg = game.LoadBitmap("char_bg3.png")
        p_btnHighlight = game.LoadBitmap("btnSelect.png")
        scrollBtn = New Sprite(p_game)
        scrollBtn.Position = New PointF(p_position.X + 2 + 483, p_position.Y + 3 + 273)
        scrollBtn.Columns = 1
        scrollBtn.AnimationRate = 0
        scrollBtn.Size = New Size(22, 30)
        scrollBtn.Image = game.LoadBitmap("ScrollBar.png")
        p_font = New Font("Narkisim", 24, FontStyle.Bold, GraphicsUnit.Pixel)
        p_font2 = New Font("Narkisim", 14, FontStyle.Regular, GraphicsUnit.Pixel)
        p_selection = 0
        p_mouseBtn = MouseButtons.None
        p_oldMouseBtn = p_mouseBtn
        p_mousePos = New Point(0, 0)
        p_visible = False
        p_lastButton = -1
        CreateInventory()
        CreateButtons()
        greenCrystal = LoadCrystalSprite(greenCrystal, "green crystal anim.png")
        redCrystal = LoadCrystalSprite(redCrystal, "Red Crystal Anim.png")
        blueCrystal = LoadCrystalSprite(blueCrystal, "Blue Crystal Anim.png")
        purpleCrystal = LoadCrystalSprite(purpleCrystal, "Purple Crystal Anim.png")
        yellowCrystal = LoadCrystalSprite(yellowCrystal, "Yellow Crystal Anim.png")
    End Sub

    Private Function LoadCrystalSprite(ByRef crystal As Sprite, filename As String) As Sprite
        Try
            crystal = New Sprite(p_game)
            crystal.Image = p_game.LoadBitmap(filename)
            crystal.TotalFrames = 9
            crystal.Columns = 3
            crystal.Size = New Size(32, 64)
            Return crystal
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Return Nothing
        End Try
    End Function

    Public Sub CreateInventory()
        For n = 0 To p_inventory.Length - 1
            p_inventory(n) = New Item()
            p_inventory(n).Name = ""
        Next
    End Sub

    Public Function AddItem(ByVal itm As Item) As Boolean
        For n = 0 To 20
            If p_inventory(n).Name = "" Then
                CopyInventoryItem(itm, p_inventory(n))
                Return True
            End If
        Next
        Return False
    End Function

    Public Function RemoveOneItem(ByVal itm As Item) As Boolean
        For n As Integer = 0 To 29 Step 1
            If p_inventory(n).Name = itm.Name Then
                    p_inventory(n).Name = ""
                    p_inventory(n).InvImageFilename = ""
                    p_buttons(n).imagefile = ""
                    p_buttons(n).image = Nothing
                Exit For
            End If
        Next
    End Function

    Public Sub RemoveAllItems(ByVal itm As Item)
        For n = 0 To 29 Step 1
            If p_inventory(n).Name = itm.Name Then
                p_inventory(n).Name = ""
                p_inventory(n).InvImageFilename = ""
                p_buttons(n).imagefile = ""
                p_buttons(n).image = Nothing
            End If
        Next
    End Sub

    Public Sub CreateButtons()
        Dim rx, ry, rw, rh As Integer
        Dim index As Integer = 0

        REM create inventory buttons
        For y = 0 To 2
            For x = 0 To 6
                rx = p_position.X + 6 + x * 76
                ry = p_position.Y + 278 + y * 76
                rw = 64
                rh = 64
                p_buttons(index).rect = New Rectangle(rx, ry, rw, rh)
                p_buttons(index).text = index.ToString()
                index += 1
            Next
        Next

        REM create left gear buttons
        rx = p_position.X + 148 + 6
        ry = p_position.Y + 22 + 3
        p_buttons(index).rect = New Rectangle(rx, ry, rw, rh)
        p_buttons(index).text = "Cape"
        index += 1

        ry += 76
        p_buttons(index).rect = New Rectangle(rx, ry, rw, rh)
        p_buttons(index).text = "Weapon 1"
        BTN_RTHAND = index
        index += 1

        ry += 76
        p_buttons(index).rect = New Rectangle(rx, ry, rw, rh)
        p_buttons(index).text = "Ring"
        index += 1

        REM create center gear buttons
        rx = p_position.X + 148 + 76 + 6
        ry = p_position.Y + 22 + 3
        p_buttons(index).rect = New Rectangle(rx, ry, rw, rh)
        p_buttons(index).text = "Helm"
        BTN_HEAD = index
        index += 1

        ry += 76
        p_buttons(index).rect = New Rectangle(rx, ry, rw, rh)
        p_buttons(index).text = "Chest"
        BTN_CHEST = index
        index += 1

        ry += 76
        p_buttons(index).rect = New Rectangle(rx, ry, rw, rh)
        p_buttons(index).text = "Boots"
        BTN_LEGS = index
        index += 1

        REM create right gear buttons
        rx = p_position.X + 143 + 76 + 76 + 11
        ry = p_position.Y + 22 + 3
        p_buttons(index).rect = New Rectangle(rx, ry, rw, rh)
        p_buttons(index).text = "Amulet"
        index += 1

        ry += 76
        p_buttons(index).rect = New Rectangle(rx, ry, rw, rh)
        p_buttons(index).text = "Weapon 2"
        BTN_LTHAND = index
        index += 1

        ry += 76
        p_buttons(index).rect = New Rectangle(rx, ry, rw, rh)
        p_buttons(index).text = "Gauntlets"
        index += 1

        REM create destroy item button
        p_buttons(index).rect = New Rectangle(p_position.X + 413 + 6, p_position.Y + 96 + 6, 76, 76)
        p_buttons(index).text = "Destroy Item"
        BTN_DESTROY = index
        index += 1

        REM create use item button
        p_buttons(index).rect = New Rectangle(p_position.X + 32 + 6, p_position.Y + 96 + 6, 76, 76)
        p_buttons(index).text = "Use Item"
        BTN_USE = index
        index += 1


        For Each btn As Button In p_buttons
            btn.bgImage = p_game.LoadBitmap("invenSquare.png")
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

    Public Property LastButton() As Integer
        Get
            Return p_lastButton
        End Get
        Set(ByVal value As Integer)
            p_lastButton = value
        End Set
    End Property

    Private Sub Print(ByVal x As Integer, ByVal y As Integer, ByVal text As String)
        Print(x, y, text, Brushes.White)
    End Sub

    Private Sub Print(ByVal x As Integer, ByVal y As Integer, ByVal text As String, ByVal color As Brush)
        p_game.Device.DrawString(text, p_font, color, x, y)
    End Sub

    REM print text right-justified from top-right x,y
    Private Sub PrintRight(ByVal x As Integer, ByVal y As Integer, ByVal text As String, ByVal color As Brush)
        Dim rsize As SizeF
        rsize = p_game.Device.MeasureString(text, p_font)
        p_game.Device.DrawString(text, p_font, color, x - rsize.Width, y)
    End Sub

    Public Sub updateMouse(ByVal mousePos As Point, ByVal mouseBtn As MouseButtons)
        p_mousePos = mousePos
        p_oldMouseBtn = p_mouseBtn
        p_mouseBtn = mouseBtn
    End Sub

    Private Sub updateScrollBar()
        'Dim mouseRect As New Rectangle(p_mousePos.X, p_mousePos.Y, 1, 1)
        'Dim scrollRect As New Rectangle(482 + p_position.X + 3, 272 + p_position.Y + 3, 23, 226)
        'scrollBtn.Draw()
        'If scrollRect.IntersectsWith(mouseRect) Then
        '    If mouseRect.IntersectsWith(scrollBtn.Bounds) Then
        '        p_game.Device.FillRectangle(New Pen(Color.FromArgb(255 * 0.2F, 255, 255, 255), 1).Brush, scrollBtn.Bounds)
        '       If p_oldMouseBtn = MouseButtons.Left Then
        '             scrollBtn.Y = mouseRect.Y
        '             If scrollBtn.Y - scrollBtn.Width > scrollRect.Y + scrollRect.Height Then scrollBtn.Y = scrollRect.Y + scrollRect.Height - scrollBtn.Width
        '              If scrollBtn.Y < scrollRect.Y Then scrollBtn.Y = scrollRect.Y
        '          End If
        '      End If
        '   End If
        '   scrollPos.X = 0.0F
        '   scrollPos.Y = scrollBtn.Y - (scrollRect.Y + scrollRect.Height)
    End Sub

    Public Sub Draw()
        If Not p_visible Then
            '    scrollPos.Y = 0
            '  scrollBtn.Y = 272 + p_position.Y + 3
            Return
        End If

        Dim tx, ty As Integer

        REM draw background 
        p_game.DrawBitmap(p_bg, p_position.X, p_position.Y)
        p_game.Device.DrawRectangle(New Pen(Color.Orange, 2.0), p_position.X - 1, p_position.Y - 1, p_bg.Width + 2, p_bg.Height + 2)

        REM draw the buttons

        REM check for button click
        For n = 0 To p_buttons.Length - 1
            Dim rect As Rectangle = p_buttons(n).rect

            REM label equip buttons
            If n > 20 And p_buttons(n).image Is Nothing Then
                Dim rsize As SizeF
                rsize = p_game.Device.MeasureString(p_buttons(n).text, p_font2)
                tx = rect.X + rect.Width / 2 - rsize.Width / 2
                ty = rect.Y - 2
                p_game.Device.DrawString(p_buttons(n).text, p_font2, Brushes.DarkGray, tx, ty)
            End If

            If rect.Contains(p_mousePos) Then

                REM print the item name
                Dim rsize As SizeF
                rsize = p_game.Device.MeasureString(p_inventory(n).Name, p_font2)
                tx = rect.X + rect.Width / 2 - rsize.Width / 2
                ty = rect.Y - (rsize.Height / 2) + 2
                p_game.Device.DrawString(p_inventory(n).Name, p_font2, Brushes.DarkGray, tx, ty)

                If p_mouseBtn = MouseButtons.None And p_oldMouseBtn = MouseButtons.Left Then
                    p_selection = n
                    If p_sourceIndex = -1 Then
                        p_sourceIndex = p_selection
                    ElseIf p_targetIndex = -1 Then
                        p_targetIndex = p_selection
                    Else
                        p_sourceIndex = p_selection
                        p_targetIndex = -1
                    End If
                    Exit For
                End If
                p_game.Device.DrawImage(p_btnHighlight, rect.X - 3, rect.Y - 3, 71, 71)
            End If
        Next

        If p_sourceIndex <> -1 And p_selection <> -1 And p_targetIndex = -1 Then
            Dim filename = p_buttons(p_sourceIndex).imagefile
            Dim img As Bitmap = p_buttons(p_sourceIndex).image
            If p_buttons(p_sourceIndex).image Is Nothing Then
            Else
                p_game.Device.DrawImage(img, New Rectangle(p_game.MousePos.X - 32, p_game.MousePos.Y - 32, 64, 64))
            End If
        End If

        If p_selection <> -1 And p_sourceIndex <> -1 And p_targetIndex <> -1 Then
            If p_buttons(p_sourceIndex).image Is Nothing Then
                CopyInventoryItem(p_inventory(p_sourceIndex), p_inventory(p_sourceIndex))
            ElseIf p_buttons(p_targetIndex).image IsNot Nothing Then
                CopyInventoryItem(p_inventory(p_sourceIndex), p_inventory(p_sourceIndex))
            Else
                MoveInventoryItem(p_sourceIndex, p_targetIndex)
                p_selection = -1
            End If
        End If

        REM draw equipment
        For n = 0 To p_inventory.Length - 1
            DrawInventoryItem(n)
        Next

        updateScrollBar()

        p_game.Update()
    End Sub

    Private Sub DrawInventoryItem(ByVal index As Integer)
        Dim filename As String
        filename = p_inventory(index).InvImageFilename
        If filename.Length > 0 Then
            REM try to avoid repeatedly loading image
            If p_buttons(index).image Is Nothing Or p_buttons(index).imagefile <> filename Then
                p_buttons(index).imagefile = filename
                p_buttons(index).image = p_game.LoadBitmap(filename)
            End If
            Dim srcRect As RectangleF = p_buttons(index).image.GetBounds(GraphicsUnit.Pixel)
            Dim dstRect As RectangleF = p_buttons(index).rect
            p_game.Device.DrawImage(p_buttons(index).image, dstRect, srcRect, GraphicsUnit.Pixel)
        End If
    End Sub

    Private Sub MoveInventoryItem(ByVal source As Integer, ByVal dest As Integer)
        If dest = BTN_DESTROY Then REM remove the item
            If p_inventory(source).Category <> "MiscAlways" AndAlso p_inventory(source).Category <> "Quest Item" Then
                p_inventory(source).Name = ""
                p_inventory(source).InvImageFilename = ""
                p_buttons(source).imagefile = ""
                p_buttons(source).image = Nothing
                Return
            End If
            Console.WriteLine("true")
        ElseIf dest = BTN_USE Then REM use the item
            If p_inventory(source).Category = "Usable Item" Then
                Select Case p_inventory(source).Name

                End Select
                p_inventory(source).Name = ""
                p_inventory(source).InvImageFilename = ""
                p_buttons(source).imagefile = ""
                p_buttons(source).image = Nothing
                Return
            End If
        End If
        Select Case p_inventory(source).Category
            Case "Misc"
                If dest = 0 Or dest = 1 Or dest = 2 Or dest = 3 Or dest = 4 Or dest = 5 Or dest = 6 Or dest = 7 Or dest = 8 Or dest = 9 Or dest = 10 Or dest = 11 Or dest = 12 Or dest = 13 Or dest = 14 Or dest = 15 Or dest = 16 Or dest = 17 Or dest = 18 Or dest = 19 Or dest = 20 Then
                    CopyInventoryItem(p_inventory(source), p_inventory(dest))
                    p_inventory(source).Name = ""
                    p_inventory(source).InvImageFilename = ""
                    p_buttons(source).imagefile = ""
                    p_buttons(source).image = Nothing
                Else
                    CopyInventoryItem(p_inventory(source), p_inventory(source))
                End If
            Case "Weapon"
                If dest = 22 Or dest = 0 Or dest = 1 Or dest = 2 Or dest = 3 Or dest = 4 Or dest = 5 Or dest = 6 Or dest = 7 Or dest = 8 Or dest = 9 Or dest = 10 Or dest = 11 Or dest = 12 Or dest = 13 Or dest = 14 Or dest = 15 Or dest = 16 Or dest = 17 Or dest = 18 Or dest = 19 Or dest = 20 Then
                    CopyInventoryItem(p_inventory(source), p_inventory(dest))
                    p_inventory(source).Name = ""
                    p_inventory(source).InvImageFilename = ""
                    p_buttons(source).imagefile = ""
                    p_buttons(source).image = Nothing
                Else
                    CopyInventoryItem(p_inventory(source), p_inventory(source))
                End If
            Case "Ring"
                If dest = 23 Or dest = 0 Or dest = 1 Or dest = 2 Or dest = 3 Or dest = 4 Or dest = 5 Or dest = 6 Or dest = 7 Or dest = 8 Or dest = 9 Or dest = 10 Or dest = 11 Or dest = 12 Or dest = 13 Or dest = 14 Or dest = 15 Or dest = 16 Or dest = 17 Or dest = 18 Or dest = 19 Or dest = 20 Then
                    CopyInventoryItem(p_inventory(source), p_inventory(dest))
                    p_inventory(source).Name = ""
                    p_inventory(source).InvImageFilename = ""
                    p_buttons(source).imagefile = ""
                    p_buttons(source).image = Nothing
                Else
                    CopyInventoryItem(p_inventory(source), p_inventory(source))
                End If
            Case "Cape"
                If dest = 21 Or dest = 0 Or dest = 1 Or dest = 2 Or dest = 3 Or dest = 4 Or dest = 5 Or dest = 6 Or dest = 7 Or dest = 8 Or dest = 9 Or dest = 10 Or dest = 11 Or dest = 12 Or dest = 13 Or dest = 14 Or dest = 15 Or dest = 16 Or dest = 17 Or dest = 18 Or dest = 19 Or dest = 20 Then
                    CopyInventoryItem(p_inventory(source), p_inventory(dest))
                    p_inventory(source).Name = ""
                    p_inventory(source).InvImageFilename = ""
                    p_buttons(source).imagefile = ""
                    p_buttons(source).image = Nothing
                Else
                    CopyInventoryItem(p_inventory(source), p_inventory(source))
                End If
            Case "Necklace"
                If dest = 27 Or dest = 0 Or dest = 1 Or dest = 2 Or dest = 3 Or dest = 4 Or dest = 5 Or dest = 6 Or dest = 7 Or dest = 8 Or dest = 9 Or dest = 10 Or dest = 11 Or dest = 12 Or dest = 13 Or dest = 14 Or dest = 15 Or dest = 16 Or dest = 17 Or dest = 18 Or dest = 19 Or dest = 20 Then
                    CopyInventoryItem(p_inventory(source), p_inventory(dest))
                    p_inventory(source).Name = ""
                    p_inventory(source).InvImageFilename = ""
                    p_buttons(source).imagefile = ""
                    p_buttons(source).image = Nothing
                Else
                    CopyInventoryItem(p_inventory(source), p_inventory(source))
                End If
            Case "Boots"
                If dest = 26 Or dest = 0 Or dest = 1 Or dest = 2 Or dest = 3 Or dest = 4 Or dest = 5 Or dest = 6 Or dest = 7 Or dest = 8 Or dest = 9 Or dest = 10 Or dest = 11 Or dest = 12 Or dest = 13 Or dest = 14 Or dest = 15 Or dest = 16 Or dest = 17 Or dest = 18 Or dest = 19 Or dest = 20 Then
                    CopyInventoryItem(p_inventory(source), p_inventory(dest))
                    p_inventory(source).Name = ""
                    p_inventory(source).InvImageFilename = ""
                    p_buttons(source).imagefile = ""
                    p_buttons(source).image = Nothing
                Else
                    CopyInventoryItem(p_inventory(source), p_inventory(source))
                End If
            Case "Armor"
                If dest = 25 Or dest = 0 Or dest = 1 Or dest = 2 Or dest = 3 Or dest = 4 Or dest = 5 Or dest = 6 Or dest = 7 Or dest = 8 Or dest = 9 Or dest = 10 Or dest = 11 Or dest = 12 Or dest = 13 Or dest = 14 Or dest = 15 Or dest = 16 Or dest = 17 Or dest = 18 Or dest = 19 Or dest = 20 Then
                    CopyInventoryItem(p_inventory(source), p_inventory(dest))
                    p_inventory(source).Name = ""
                    p_inventory(source).InvImageFilename = ""
                    p_buttons(source).imagefile = ""
                    p_buttons(source).image = Nothing
                Else
                    CopyInventoryItem(p_inventory(source), p_inventory(source))
                End If
            Case "Helm"
                If dest = 24 Or dest = 0 Or dest = 1 Or dest = 2 Or dest = 3 Or dest = 4 Or dest = 5 Or dest = 6 Or dest = 7 Or dest = 8 Or dest = 9 Or dest = 10 Or dest = 11 Or dest = 12 Or dest = 13 Or dest = 14 Or dest = 15 Or dest = 16 Or dest = 17 Or dest = 18 Or dest = 19 Or dest = 20 Then
                    CopyInventoryItem(p_inventory(source), p_inventory(dest))
                    p_inventory(source).Name = ""
                    p_inventory(source).InvImageFilename = ""
                    p_buttons(source).imagefile = ""
                    p_buttons(source).image = Nothing
                Else
                    CopyInventoryItem(p_inventory(source), p_inventory(source))
                End If
            Case "Hands"
                If dest = 29 Or dest = 0 Or dest = 1 Or dest = 2 Or dest = 3 Or dest = 4 Or dest = 5 Or dest = 6 Or dest = 7 Or dest = 8 Or dest = 9 Or dest = 10 Or dest = 11 Or dest = 12 Or dest = 13 Or dest = 14 Or dest = 15 Or dest = 16 Or dest = 17 Or dest = 18 Or dest = 19 Or dest = 20 Then
                    CopyInventoryItem(p_inventory(source), p_inventory(dest))
                    p_inventory(source).Name = ""
                    p_inventory(source).InvImageFilename = ""
                    p_buttons(source).imagefile = ""
                    p_buttons(source).image = Nothing
                Else
                    CopyInventoryItem(p_inventory(source), p_inventory(source))
                End If
            Case "Sheild"
                If dest = 28 Or dest = 0 Or dest = 1 Or dest = 2 Or dest = 3 Or dest = 4 Or dest = 5 Or dest = 6 Or dest = 7 Or dest = 8 Or dest = 9 Or dest = 10 Or dest = 11 Or dest = 12 Or dest = 13 Or dest = 14 Or dest = 15 Or dest = 16 Or dest = 17 Or dest = 18 Or dest = 19 Or dest = 20 Then
                    CopyInventoryItem(p_inventory(source), p_inventory(dest))
                    p_inventory(source).Name = ""
                    p_inventory(source).InvImageFilename = ""
                    p_buttons(source).imagefile = ""
                    p_buttons(source).image = Nothing
                Else
                    CopyInventoryItem(p_inventory(source), p_inventory(source))
                End If
        End Select
    End Sub

    Public Sub CopyInventoryItem(ByVal source As Integer, ByVal dest As Integer)
        CopyInventoryItem(p_inventory(source), p_inventory(dest))
    End Sub

    Public Sub CopyInventoryItem(ByRef srcItem As Item, ByRef dstItem As Item)
        dstItem.Name = srcItem.Name
        dstItem.Description = srcItem.Description
        dstItem.AttackDie = srcItem.AttackDie
        dstItem.AttackNumDice = srcItem.AttackNumDice
        dstItem.Category = srcItem.Category
        dstItem.Defense = srcItem.Defense
        dstItem.DropImageFilename = srcItem.DropImageFilename
        dstItem.InvImageFilename = srcItem.InvImageFilename
        dstItem.Value = srcItem.Value
        dstItem.Weight = srcItem.Weight
        dstItem.STR = srcItem.STR
        dstItem.DEX = srcItem.DEX
        dstItem.CHA = srcItem.CHA
        dstItem.STA = srcItem.STA
        dstItem.INT = srcItem.INT
    End Sub

    REM see if player has item in inventory
    Public Function HasItem(ByVal name As String) As Boolean
        Dim count As Integer = ItemCount(name)
        If count = 0 Then
            Return False
        Else
            Return True
        End If
    End Function

    Public Function HasItem(itm As Item) As Boolean
        For Each it As Item In p_inventory
            If it IsNot Nothing Then
                If it Is itm Then
                    Return True
                End If
            End If
        Next
        Return False
    End Function

    REM count number of this item in inventory 
    Public Function ItemCount(ByVal name As String) As Integer
        Dim count As Integer = 0
        For Each it As Item In p_inventory
            If it IsNot Nothing Then
                If name = it.Name Then count += 1
            End If
        Next
        Return count
    End Function

    REM return an item by index # (not reliable)
    Public Function GetItem(ByVal index As Integer) As Item
        If index < 0 Then
            index = 0
        ElseIf index > p_inventory.Length() - 1 Then
            index = p_inventory.Length() - 1
        End If
        Return p_inventory(index)
    End Function

    REM forcibly change item 
    Public Sub SetItem(ByVal itm As Item, ByVal index As Integer)
        If index < 0 Then
            index = 0
        ElseIf index > p_inventory.Length - 1 Then
            index = p_inventory.Length - 1
        End If
        p_inventory(index) = itm
    End Sub

    Public Sub ClearInventory()
        For n As Integer = 0 To p_inventory.Count - 1
            p_inventory(n).Name = ""
            p_inventory(n).InvImageFilename = ""
            p_buttons(n).imagefile = ""
            p_buttons(n).image = Nothing
        Next
    End Sub

End Class