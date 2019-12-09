Public Class Wounded_Farmer
    Inherits NPC
    Private game As Game
    Public Sub New(ByRef game As Game)
        MyBase.New(game)
        Me.Load("Wounded Farmer.char")
        Load("Wounded Farmer.char")
        Position = New Point(1280, 288)
        range = New Rectangle(1280, 288, 150, 150)
        Alive = True
        AnimationState = Character.AnimationStates.Walking
        Direction = 4
    End Sub
    Public Overrides Sub Update(ByRef dialog As Dialogue)
        Dim relativePos As PointF
        Dim heroCenter As PointF
        Dim npcCenter As PointF
        Dim dist As Single
        Static target As Integer = 0
        Dim index As Integer = 0

        Try
            If game.inven.Visible Then Return
        Catch ex As Exception
        End Try

        If X > game.world.ScrollPos.X - 64 _
        And X < game.world.ScrollPos.X + 23 * 32 + 64 _
        And Y > game.world.ScrollPos.Y - 54 _
        And Y < game.world.ScrollPos.Y + 17 * 32 Then
            heroCenter = game.hero.CenterPos
            relativePos.X = X - game.world.ScrollPos.X
            relativePos.Y = Y - game.world.ScrollPos.Y
            Draw(relativePos)

            REM get center of NPC
            npcCenter = relativePos
            npcCenter.X += GetSprite.Width / 2
            npcCenter.Y += GetSprite.Height / 2

            REM get distance to the NPC
            dist = game.hero.CenterDistance(npcCenter)

            REM is player trying to talk to this NPC?
            If dist < talkRadius And Alive Then
                game.Device.DrawEllipse(New Pen(Color.Blue, 2.0), npcCenter.X - 24, npcCenter.Y, 48, 48)
                TalkFlag = True
                If game.casualVisible = False Then
                    game.p_dialog = game.Random(0, 4)
                End If
                game.casualVisible = True
            Else
                game.casualVisible = False
                TalkFlag = False
            End If
        Else
            TalkFlag = False
        End If

        If TalkFlag Then
            TalkFlag = False
            game.Peasant.Moving = False

            REM make monster face the player
            Dim dir As Integer
            dir = game.combat.GetTargetDirection(npcCenter, game.hero.CenterPos)
            If game.Peasant.AnimationState <> Character.AnimationStates.Dead Then
                game.Peasant.Direction = dir
                game.Peasant.AnimationState = Character.AnimationStates.Talking
            End If

            REM make player face the NPC
            dir = game.combat.GetTargetDirection(game.hero.CenterPos, npcCenter)
            If game.Peasant.AnimationState <> Character.AnimationStates.Dead Then
                game.hero.AnimationState = Character.AnimationStates.Talking
                game.hero.Direction = dir
            End If
            Static advance As Boolean = False
            Static questIncreased As Boolean = False
            Select Case game.quests.QuestNumber
                Case 4
                    If game.Peasant.AnimationState <> Character.AnimationStates.Dead Then
                        dialog.Title = "Wounded Farmer"
                        dialog.Message = "Huh? Who are you? Are you a celtic? Wha..The village? I'm not entirely sure... It " + _
                            "all happened so quick. I remember something about bandits. Ah, yes! It was bandits. They pillaged the village. We couldn't stop " + _
                            "them. We're just honest farm folk. None of us know how to fight. I hope you can catch them. I think they headed due west. " + _
                            "...I wish I could be of more use. Unh!"
                        dialog.NumButtons = 1
                        dialog.setButtonText(1, "Next")
                        dialog.setCorner(Dialogue.Positions.UpperLeft)
                        dialog.Visible = True
                        If dialog.Selection = 1 Then
                            game.Peasant.AnimationState = Character.AnimationStates.Dead
                            dialog.Visible = False
                            game.hero.Frozen = False
                            game.hero.iceSprite.Image = game.LoadBitmap("frozenmagic.png")
                            advance = True
                        Else
                            If game.quests.QuestNumber <> 4 Then game.quests.QuestNumber = 4
                            game.hero.Frozen = True
                            game.hero.iceSprite.Image = game.LoadBitmap("blank square.png")
                            game.hero._freezeTime = 10
                        End If
                    End If
                Case Else
                    If questIncreased Then
                        Form1.showDialogTutorialKnight1("Wounded Farmer", "He's dead. There is nothing left to see.", 1, "Okay")
                        If dialog.Selection = 1 Then
                            dialog.Visible = False
                            game.hero.Frozen = False
                            game.hero.iceSprite.Image = game.LoadBitmap("frozenmagic.png")
                        Else
                            game.hero.Frozen = True
                            game.hero.iceSprite.Image = game.LoadBitmap("blank square.png")
                            game.hero._freezeTime = 10
                        End If
                    End If
            End Select
            If Not advance Then
                If game.quests.QuestNumber <> 4 Then game.quests.QuestNumber = 4
            Else
                If Not questIncreased Then
                    questIncreased = True
                    game.quests.QuestNumber += 1
                End If
            End If
        Else
            If game.Peasant.AnimationState <> Character.AnimationStates.Dead Then
                game.Peasant.AnimationState = Character.AnimationStates.Standing
            End If
        End If

        MyBase.Update()
    End Sub
End Class