Public Class Combat
    Public killcount As Integer
    Private p_attackFlag As Boolean
    Private p_target As Character
    Private p_attackText As String
    Private p_attackDamage As Integer
    Private p_game As Game
    Private frameCount As Integer = 0
    Private font As Font
    Public Sub New(ByRef game As Game)
        p_game = game
        p_attackText = ""
        p_attackDamage = 0
        p_attackFlag = False
        font = New Font("Arial Bold", 20, FontStyle.Regular, GraphicsUnit.Pixel)
    End Sub
    Public Property Target() As Character
        Get
            Return p_target
        End Get
        Set(ByVal value As Character)
            p_target = value
        End Set
    End Property
    Public Property AttackText() As String
        Get
            Return p_attackText
        End Get
        Set(ByVal value As String)
            p_attackText = value
        End Set
    End Property
    Public Property AttackDamage() As Integer
        Get
            Return p_attackDamage
        End Get
        Set(ByVal value As Integer)
            p_attackDamage = value
        End Set
    End Property
    Public Function GetTargetDirection(ByVal source As PointF, ByVal target As PointF) As Integer
        Dim direction As Integer = 0
        If source.X < target.X - 16 Then
            If source.Y < target.Y - 8 Then
                direction = 5 'south east
            ElseIf source.Y > target.Y + 8 Then
                direction = 2 'north east
            Else
                direction = 0 'east
            End If
        ElseIf source.X > target.X + 16 Then
            If source.Y < target.Y - 8 Then
                direction = 6 'south west
            ElseIf source.Y > target.Y + 8 Then
                direction = 3 'north west
            Else
                direction = 7 'west
            End If
        Else
            If source.Y < target.Y - 8 Then
                direction = 4 'south
            ElseIf source.Y > target.Y + 8 Then
                direction = 1 'north
            End If
        End If
        Return direction
    End Function
    Public Function GetFaceableDirections(ByVal source As PointF, ByVal target As PointF) As List(Of Integer)
        Dim directions As New List(Of Integer)
        If source.X < target.X - 16 Then
            If source.Y < target.Y - 8 Then
                'south east
                directions.Add(5)
                directions.Add(0)
                directions.Add(4)
            ElseIf source.Y > target.Y + 8 Then
                'north east
                directions.Add(2)
                directions.Add(1)
                directions.Add(0)
            Else
                directions.Add(0) 'east
                directions.Add(2)
                directions.Add(5)
            End If
        ElseIf source.X > target.X + 16 Then
            If source.Y < target.Y - 8 Then
                directions.Add(6) 'south west
                directions.Add(4)
                directions.Add(7)
            ElseIf source.Y > target.Y + 8 Then
                directions.Add(3) 'north west
                directions.Add(7)
                directions.Add(1)
            Else
                directions.Add(7) 'west
                directions.Add(3)
                directions.Add(6)
            End If
        Else
            If source.Y < target.Y - 8 Then
                directions.Add(4) 'south
                directions.Add(6)
                directions.Add(5)
            ElseIf source.Y > target.Y + 8 Then
                directions.Add(1) 'north
                directions.Add(3)
                directions.Add(2)
            End If
        End If
        Return directions
    End Function
    Public Function RotateTo(x As Single, y As Single, x2 As Single, y2 As Single) As PointF
        Dim pos As PointF
        pos = New PointF(x - x2, y - y2)
        pos.X /= pos.X
        pos.Y /= pos.Y
        If pos.X < 0 Then pos.X *= -1
        If pos.Y < 0 Then pos.Y *= -1
        Return pos
    End Function
    Public Sub DropLoot(ByRef srcMonster As Character)
        Dim itm As Item
        Dim p As Point
        Dim count As Integer
        Dim rad As Integer = 64

        REM any gold to drop?
        itm = New Item()
        Dim gold As Integer
        gold = p_game.Random(srcMonster.DropGoldMin, srcMonster.DropGoldMax)
        p.X = srcMonster.X + p_game.Random(rad) + rad / 2
        p.Y = srcMonster.Y + p_game.Random(rad) + rad / 2
        DropGold(gold, p.X, p.Y)


        REM any items to drop?

        If srcMonster.DropNum1 > 0 And srcMonster.DropItem1 <> "" Then
            count = p_game.Random(1, srcMonster.DropNum1)
            For n = 1 To count
                If p_game.items.GetItem(srcMonster.DropItem1).Category = "MiscAlways" Then
                    REM 100% chance for drop
                    itm = p_game.items.GetItem(srcMonster.DropItem1)
                    p.X = srcMonster.X + p_game.Random(rad) - rad / 2
                    p.Y = srcMonster.Y + p_game.Random(rad) - rad / 2
                    DropTreasureItem(itm, p.X, p.Y)
                Else
                    REM 25% chance for drop
                    If p_game.Random(100) < 25 Then
                        itm = p_game.items.GetItem(srcMonster.DropItem1)
                        p.X = srcMonster.X + p_game.Random(rad) - rad / 2
                        p.Y = srcMonster.Y + p_game.Random(rad) - rad / 2
                        DropTreasureItem(itm, p.X, p.Y)
                    End If
                End If
            Next
        End If
        If srcMonster.DropNum2 > 0 And srcMonster.DropItem2 <> "" Then
            count = p_game.Random(1, srcMonster.DropNum2)
            For n = 1 To count
                If p_game.items.GetItem(srcMonster.DropItem2).Category = "MiscAlways" Then
                    REM 100% chance for drop
                    itm = p_game.items.GetItem(srcMonster.DropItem2)
                    p.X = srcMonster.X + p_game.Random(rad) - rad / 2
                    p.Y = srcMonster.Y + p_game.Random(rad) - rad / 2
                    DropTreasureItem(itm, p.X, p.Y)
                Else
                    REM 25% chance for drop
                    If p_game.Random(100) < 25 Then
                        itm = p_game.items.GetItem(srcMonster.DropItem2)
                        p.X = srcMonster.X + p_game.Random(rad) - rad / 2
                        p.Y = srcMonster.Y + p_game.Random(rad) - rad / 2
                        DropTreasureItem(itm, p.X, p.Y)
                    End If
                End If
            Next
        End If
        If srcMonster.DropNum3 > 0 And srcMonster.DropItem3 <> "" Then
            count = p_game.Random(1, srcMonster.DropNum3)
            For n = 1 To count
                If p_game.items.GetItem(srcMonster.DropItem3).Category = "MiscAlways" Then
                    REM 100% chance for drop
                    itm = p_game.items.GetItem(srcMonster.DropItem3)
                    p.X = srcMonster.X + p_game.Random(rad) - rad / 2
                    p.Y = srcMonster.Y + p_game.Random(rad) - rad / 2
                    DropTreasureItem(itm, p.X, p.Y)
                Else
                    REM 25% chance for drop
                    If p_game.Random(100) < 25 Then
                        itm = p_game.items.GetItem(srcMonster.DropItem3)
                        p.X = srcMonster.X + p_game.Random(rad) - rad / 2
                        p.Y = srcMonster.Y + p_game.Random(rad) - rad / 2
                        DropTreasureItem(itm, p.X, p.Y)
                    End If
                End If
            Next
        End If
    End Sub
    Public Sub DropGold(ByVal amount As Integer, ByVal x As Integer, ByVal y As Integer)
        Dim itm As New Item()
        itm.Name = "gold"
        itm.DropImageFilename = "gold.png"
        itm.InvImageFilename = "gold.png"
        itm.Value = amount
        DropTreasureItem(itm, x, y)
    End Sub
    Public Sub DropTreasureItem(ByRef itm As Item, ByVal x As Integer, ByVal y As Integer)
        Dim drit As Game.DrawableItem
        drit.item = itm

        drit.sprite = New Sprite(p_game)
        drit.sprite.Position = New Point(x, y)

        If drit.item.DropImageFilename = "" Then
            MessageBox.Show("Error: Item '" + drit.item.Name + "' image file is invalid.")
            End
        End If

        drit.sprite.Image = p_game.LoadBitmap(drit.item.DropImageFilename)
        drit.sprite.Size = drit.sprite.Image.Size

        p_game.treasure.Add(drit)
    End Sub
    Public Sub CheckEnemyDeath()
        Dim text As String = ""

        If p_target IsNot Nothing Then
            REM is monster dead?
            Dim oldTarget As New Character(p_game)
            If p_target.Health <= 0 Then
                p_target.Alive = False

                If oldTarget IsNot p_target Then
                    oldTarget = p_target
                    p_target.AnimationState = Character.AnimationStates.Dying
                End If

                If p_game.quests.QuestItem.RequiredKillNPCFlag Then
                    If p_target.Name = p_game.quests.QuestItem.RequiredKillNPC Then
                        killcount += 1
                        If killcount >= p_game.quests.QuestItem.RequiredKillCount Then
                            p_game.quests.QuestItem.RequiredKillNPCFlag = False
                            killcount = 0
                        End If
                    End If
                End If
                Dim xp As Integer
                xp = p_game.hero.AddCombatXP(p_target)

                REM do looting
                p_game.hero.AnimationState = Character.AnimationStates.Standing
                DropLoot(p_target)
                p_target = Nothing
            End If
        End If
    End Sub
    Public Sub CheckPlayerDeath()
        REM is player dead?
        If p_game.hero.Health <= 0 Then
            p_game.hero.Alive = False
            p_game.hero.AnimationState = Character.AnimationStates.Dying
        End If
    End Sub
    Public Sub EnemyAttack()

        PlayEnemyAttack(p_target)

        If p_game.hero.Health <= 0 Then Return

        Static text As String = ""
        Dim hit As Boolean = False
        Dim critical As Boolean = False
        Dim fail As Boolean = False
        Dim roll As Integer = 0
        Dim AC As Integer = 0
        Dim pos As New Point
        Dim sz As New SizeF

        REM calculate player's AC (NPC is attacking)
        AC = p_game.hero.DEX + p_game.hero.GetArmorValue()

        REM calculate chance to-hit for NPC
        roll = p_game.Random(1, 20)
        If roll = 20 Then
            REM critical hit!
            hit = True
            critical = True
        ElseIf roll = 1 Then
            fail = True
        Else
            REM normal hit
            roll += p_target.STR
            If roll > AC Then hit = True
        End If
        REM did attack succeed?
        If hit Then
            REM calculate base damage
            AttackDamage = p_game.Random(1, 4)

            REM add critical
            If critical Then AttackDamage *= 2

            REM add STR
            AttackDamage += p_target.STR

            REM add weapon damage 
            Dim wpn As Integer = p_game.Random(1, 4) * p_target.Level + 1
            AttackDamage += wpn

            REM subtract AC
            AttackDamage -= AC

            REM minimal hit
            If AttackDamage < 1 Then AttackDamage = 1

            REM show result
            sz = p_game.Device.MeasureString(AttackDamage.ToString, font)
            p_game.AddFadingText(p_game.hero.GetSprite.X + (p_game.hero.GetSprite().Width / 2) - (sz.Width / 2), p_game.hero.GetSprite().Y - 20, AttackDamage.ToString, Color.Red, 0.5F, New PointF(0, -2.0F))
            p_game.hero.Health -= AttackDamage
            DoElementalDamage(p_target, p_game.hero, AttackDamage)
            PlayDamage()
        Else
            PlayEvade()
            sz = p_game.Device.MeasureString("Miss!", font)
            p_game.AddFadingText(Target.GetSprite.X + (Target.GetSprite().Width / 2) - (sz.Width / 2), Target.GetSprite().Y - 20, "Miss!", Color.Blue, 0.5F, New PointF(0, -2.0F))
        End If
        REM is player dead?
        If p_game.hero.Health <= 0 Then
            p_game.hero.Alive = False
            p_game.hero.AnimationState = Character.AnimationStates.Dying
        End If
    End Sub
    Public Sub PlayerAttack()
        Static text As String = ""
        Dim hit As Boolean = False
        Dim critical As Boolean = False
        Dim fail As Boolean = False
        Dim roll As Integer = 0
        Dim AC As Integer = 0
        Dim pos As New Point
        Dim sz As New SizeF

        If (p_game.hero.PlayerClass = "Warrior" Or p_game.hero.PlayerClass = "Paladin") Then
            Dim sucess As Boolean = False
            For Each direction As Integer In GetFaceableDirections(p_game.hero.CenterPos, p_target.CenterPos)
                If p_game.hero.Direction = direction Then
                    sucess = True
                    Exit For
                End If
            Next
            If Not sucess Then Return
        End If

        REM calculate target's AC
        AC = p_target.DEX

        REM calculate chance to-hit for PC
        roll = p_game.Random(1, 20)
        If roll = 20 Then
            REM critical hit!
            hit = True
            critical = True
        ElseIf roll = 1 Then
            fail = True
        Else
            REM normal hit
            roll += p_game.hero.STR
            If roll > AC Then hit = True

        End If

        REM did attack succeed?
        If hit Then

            REM calculate base damage
            AttackDamage = p_game.hero.GetWeaponDamage()

            REM add critical
            If critical Then AttackDamage *= 2

            REM add STR
            AttackDamage += p_game.hero.STR

            REM subtract AC
            AttackDamage -= AC

            REM minimal hit
            If AttackDamage < 1 Then AttackDamage = 1

            REM show result
            sz = p_game.Device.MeasureString(AttackDamage.ToString, font)
            p_game.AddFadingText(Target.GetSprite.X + (Target.GetSprite().Width / 2) - (sz.Width / 2) - p_game.world.X, Target.GetSprite.Y - 20 - p_game.world.Y, AttackDamage.ToString, Color.Red, 0.5F, New PointF(0, -2.0F))
            p_target.Health -= AttackDamage
            DoElementalDamage(p_game.hero, p_target, AttackDamage)
            PlayDamage()
        Else
            sz = p_game.Device.MeasureString("Miss!", font)
            PlayEvade()
            p_game.AddFadingText(p_game.hero.GetSprite.X + (p_game.hero.GetSprite().Width / 2) - (sz.Width / 2), p_game.hero.GetSprite.Y - 20, "Miss!", Color.Blue, 0.5F, New PointF(0, -2.0F))
        End If


        REM is monster dead?
        Dim oldTarget As New Character(p_game)
        If p_target.Health <= 0 Then
            p_target.Alive = False

            If oldTarget IsNot p_target Then
                oldTarget = p_target
                p_target.AnimationState = Character.AnimationStates.Dying
            End If

            If p_game.quests.QuestItem.RequiredKillNPCFlag Then
                If p_target.Name = p_game.quests.QuestItem.RequiredKillNPC Then
                    killcount += 1
                    If killcount >= p_game.quests.QuestItem.RequiredKillCount Then
                        p_game.quests.QuestItem.RequiredKillNPCFlag = False
                        killcount = 0
                    End If
                End If
            End If
            Dim xp As Integer
            xp = p_game.hero.AddCombatXP(p_target)
            Dim sz2 As SizeF = p_game.Device.MeasureString(xp.ToString + " xp", font)
            p_game.AddFadingText(p_game.hero.GetSprite.X + (p_game.hero.GetSprite().Width / 2) - (sz.Width / 2), p_game.hero.GetSprite.Y - 20, _
                xp.ToString + " xp", 1.0F, Color.YellowGreen, font, True, New PointF(0, -2.0F))

            REM do looting
            p_game.hero.AnimationState = Character.AnimationStates.Standing
            DropLoot(p_target)
            p_target = Nothing

        End If
    End Sub
    Private Sub fixElementalAttack(ByRef npc As Character)
        Dim e As String = npc.Element
        If e = "fire" Then
            e = "Fire"
        ElseIf e = "water" Or e = "Ice" Then
            e = "Thunder"
        ElseIf e = "holy" Then
            e = "Holy"
        ElseIf e = "thunder" Then
            e = "Thunder"
        ElseIf e = "dark" Then
            e = "Dark"
        End If
        npc.Element = e
    End Sub
    Private Sub DoElementalDamage(ByRef attacker As Character, ByRef victim As Character, ByVal attackDamage As Single)
        REM unstun the enemy
        If victim.Frozen Then victim.Frozen = False
        If p_game.Random(1, 100) <= 10 + (attacker.INT / 10) Then
            fixElementalAttack(attacker)
            Dim victimPos As PointF = p_game.ToScreen(victim.Position)
            Dim attackerPos As PointF = p_game.ToScreen(attacker.Position)
            Select Case attacker.Element
                Case "Holy"
                    If attacker.Regenerating = False Then
                        Dim drainAmt As Single = CSng(0.02F * (victim.INT / p_game.Random(1, 8)))
                        attacker.Regenerating = True
                        attacker.regenPower = drainAmt
                        attacker.drainVictim = victim
                        PlayHeal()
                        p_game.AddAnimationStrip(New Rectangle(attackerPos.X, attackerPos.Y, attacker.GetSprite.Width, attacker.GetSprite.Height), _
                            "heal5.png", 192, 192, 5, 13, 22)
                    End If
                Case "Thunder"
                    If victim.Frozen = False Then
                        victim.Frozen = True
                        PlayFreeze()
                        p_game.AddAnimationStrip(New Rectangle(victimPos.X, victimPos.Y, victim.GetSprite.Width, victim.GetSprite.Height), _
                            "Thunder3.png", 192, 192, 5, 5, 8)
                    End If
                Case "Fire"
                    If victim.Bleeding = False Then
                        victim.Bleeding = True
                        victim.bleedPower = CSng(0.02F * (p_target.INT / p_game.Random(1, 8)))
                        victim._bleedTime -= 5000.0F
                        p_game.AddAnimationStrip(New Rectangle(victimPos.X, victimPos.Y, victim.GetSprite.Width, victim.GetSprite.Height), _
                            "fireMagic.png", 192, 192, 5, 21, 35)
                    End If
                Case "Dark"
                    p_target.Health -= CSng((0.02F * victim.INT) * attackDamage)
                    p_game.AddAnimationStrip(New Rectangle(victimPos.X, victimPos.Y, victim.GetSprite.Width, victim.GetSprite.Height), _
                        "Darkness1.png", 192, 192, 5, 9, 15)
            End Select
        End If
    End Sub
End Class