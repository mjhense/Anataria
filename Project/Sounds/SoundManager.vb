Module SoundManager
    Private Theme As New MediaPlayer.MediaPlayer
    Private Bow As New MediaPlayer.MediaPlayer
    Private ArrowHit As New MediaPlayer.MediaPlayer
    Private Sword As New MediaPlayer.MediaPlayer
    Private Heal As New MediaPlayer.MediaPlayer
    Private enterDoor As New MediaPlayer.MediaPlayer
    Private Chest As New MediaPlayer.MediaPlayer
    Private Fire As New MediaPlayer.MediaPlayer
    Private Rain As New MediaPlayer.MediaPlayer
    Private Storm As New MediaPlayer.MediaPlayer
    Private Water As New MediaPlayer.MediaPlayer
    Private Wind As New MediaPlayer.MediaPlayer
    Private LootSound As New MediaPlayer.MediaPlayer
    Private Equip As New MediaPlayer.MediaPlayer
    Private ButtonSwitch As New MediaPlayer.MediaPlayer
    Private Switch As New MediaPlayer.MediaPlayer
    Private Damage As New MediaPlayer.MediaPlayer
    Private GameOver As New MediaPlayer.MediaPlayer
    Private Cow As New MediaPlayer.MediaPlayer
    Private Pig As New MediaPlayer.MediaPlayer
    Private Chicken As New MediaPlayer.MediaPlayer
    Private Sheep As New MediaPlayer.MediaPlayer
    Private Horse As New MediaPlayer.MediaPlayer
    Private Freeze As New MediaPlayer.MediaPlayer
    Private Posion As New MediaPlayer.MediaPlayer
    Private Cursor2 As New MediaPlayer.MediaPlayer
    Private Decision3 As New MediaPlayer.MediaPlayer
    Private Lava As New MediaPlayer.MediaPlayer
    Private levelUp As New MediaPlayer.MediaPlayer
    Private magicFire As New MediaPlayer.MediaPlayer
    Private magicHit As New MediaPlayer.MediaPlayer
    Private evade As New MediaPlayer.MediaPlayer
    Private teleport As New MediaPlayer.MediaPlayer
    Private leaveRoom As New MediaPlayer.MediaPlayer
    Private coin As New MediaPlayer.MediaPlayer
    Private Load As New MediaPlayer.MediaPlayer
    Private SaveSound As New MediaPlayer.MediaPlayer
    Private OpenMetalDoor As New MediaPlayer.MediaPlayer
    Private OpenDoorKnob As New MediaPlayer.MediaPlayer
    Private drips As New MediaPlayer.MediaPlayer
    Private darkness As New MediaPlayer.MediaPlayer
    Private decision1 As New MediaPlayer.MediaPlayer

    REM ********************************************
    REM sound glitches may be occuring due to the same
    REM sound being played while it is still running.
    REM For example, both the enemy and the player could
    REM play the damage sound at the same time
    REM ********************************************

    Public Sub Initialize()
        evade.FileName = "Sounds\Evade.mp3"
        evade.Stop()
        Bow.FileName = "Sounds\Bow.mp3"
        Bow.Stop()
        ArrowHit.FileName = "Sounds\Arrow Crash.mp3"
        ArrowHit.Stop()
        Sword.FileName = "Sounds\Sword.mp3"
        Sword.Stop()
        Damage.FileName = "Sounds\Damage.mp3"
        Damage.Stop()
        Freeze.FileName = "Sounds\Ice.mp3"
        Freeze.Stop()
        enterDoor.FileName = "Sounds\OpenDoor.mp3"
        enterDoor.Stop()
        Heal.FileName = "Sounds\Health.mp3"
        Heal.Stop()
        levelUp.FileName = "Sounds\Level Up.mp3"
        levelUp.Stop()
        Lava.FileName = "Sounds\laval.mp3"
        Lava.Volume = 0
        Lava.Stop()
        Water.FileName = "Sounds\Water2.mp3"
        Water.Volume = 0
        Water.Stop()
        GameOver.FileName = "Sounds\Gameover.mp3"
        GameOver.Stop()
        magicFire.FileName = "Sounds\MagicFire.mp3"
        magicFire.Stop()
        Fire.FileName = "Sounds\Fire.mp3"
        Fire.Volume = Fire.Volume / 0.8F
        Fire.Stop()
        LootSound.FileName = "Sounds\Loot.mp3"
        LootSound.Stop()
        Equip.FileName = "Equip.mp3"
        Equip.Stop()
        magicHit.FileName = "Sounds\magicHit.mp3"
        magicHit.Stop()
        teleport.Open("Sounds\Teleport.mp3")
        teleport.Stop()
        leaveRoom.Open("Sounds\Close.mp3")
        leaveRoom.Stop()
        Chest.Open("Sounds\Chest.mp3")
        Chest.Stop()
        coin.FileName = "Sounds\Coin.mp3"
        coin.Stop()
        Theme.FileName = "Sounds\Theme.mp3"
        Theme.Stop()
        SaveSound.FileName = "Sounds\Save.mp3"
        SaveSound.Stop()
        Load.FileName = "Sounds\Load.mp3"
        Load.Stop()
        OpenMetalDoor.FileName = "Sounds\OpenDoorMetal.mp3"
        OpenMetalDoor.Stop()
        OpenDoorKnob.FileName = "Sounds\OpenDoorHandle.mp3"
        OpenDoorKnob.Stop()
        Sheep.FileName = "Sounds\sheep.mp3"
        Sheep.Stop()
        Chicken.FileName = "Sounds\chicken.mp3"
        Chicken.Stop()
        Cow.FileName = "Sounds\Cow.mp3"
        Cow.Stop()
        Pig.FileName = "Sounds\Pig.mp3"
        Pig.Stop()
        Switch.FileName = "Sounds\Equip.mp3"
        Switch.Stop()
        drips.FileName = "Sounds\Drips.mp3"
        drips.Stop()
        darkness.FileName = "Sounds\Darkness.mp3"
        darkness.Stop()
        Decision3.FileName = "Sounds\Decision3.mp3"
        Decision3.Stop()
        Cursor2.FileName = "Sounds\Cursor2.mp3"
        Cursor2.Stop()
        decision1.FileName = "Sounds\Decision1.mp3"
        decision1.Stop()
    End Sub
    Public Function PlayDecision1()
        decision1.Play()
    End Function
    Public Function PlayCursor2()
        Cursor2.Play()
    End Function
    Public Function PlayDecision3()
        Decision3.Play()
    End Function
    Public Function PlayDarkness()
        If darkness.PlayState <> MediaPlayer.MPPlayStateConstants.mpPlaying Then darkness.Play()
    End Function
    Public Function StopDarkness()
        darkness.Stop()
    End Function
    Public Function StopDrips()
        drips.Stop()
    End Function
    Public Function PlayDrips()
        If drips.PlayState <> MediaPlayer.MPPlayStateConstants.mpPlaying Then drips.Play()
    End Function
    Public Function PlaySwitch()
        Switch.Play()
    End Function
    Public Function PlaySheep()
        Sheep.Play()
    End Function
    Public Function PlaySave()
        SaveSound.Play()
    End Function
    Public Function PlayLoad()
        Load.Play()
    End Function
    Public Function PlayTheme()
        If Theme.PlayState <> MediaPlayer.MPPlayStateConstants.mpPlaying Then Theme.Play()
    End Function
    Public Function StopTheme()
        Theme.Stop()
    End Function
    Public Function PlayCoin()
        coin.Play()
    End Function
    Public Function PlayChest()
        Chest.Play()
    End Function
    Public Function PlayTeleport()
        teleport.Play()
    End Function
    Public Function PlayEvade()
        evade.Play()
    End Function
    Public Function PlayBow()
        Bow.Play()
    End Function
    Public Function PlayArrowHit()
        ArrowHit.Play()
    End Function
    Public Function PlayLootSound()
        LootSound.Play()
    End Function
    Public Function PlayEquip()
        Equip.Play()
    End Function
    Public Function PlayFire()
        If Fire.PlayState <> MediaPlayer.MPPlayStateConstants.mpPlaying Then Fire.Play()
    End Function
    Public Function PlayWater()
        If Water.PlayState <> MediaPlayer.MPPlayStateConstants.mpPlaying Then Water.Play()
    End Function
    Public Function PlayLava()
        If Lava.PlayState <> MediaPlayer.MPPlayStateConstants.mpPlaying Then
            Lava.Play()
        End If
    End Function
    Public Function StopLava()
        Lava.Stop()
    End Function
    Public Function StopFire()
        Fire.Stop()
    End Function
    Public Function StopWater()
        Water.Stop()
    End Function
    Public Function PlayHeal()
        Heal.Play()
    End Function
    Public Function PlayFreeze()
        Freeze.Play()
    End Function
    Public Function PlayDamage()
        Damage.Play()
    End Function
    Public Function PlaySword()
        Sword.Play()
    End Function
    Public Function PlayFireMagic()
        magicFire.Play()
    End Function
    Public Function PlayLevelUp()
        levelUp.Play()
    End Function
    Public Function PlayGameOver()
        If GameOver.PlayState <> MediaPlayer.MPPlayStateConstants.mpPlaying Then GameOver.Play()
    End Function
    Public Function PlayMagicHit()
        magicHit.Play()
    End Function
    Public Function StopGameOver()
        GameOver.Stop()
    End Function
    Public Function StopSound(ByRef sound As MediaPlayer.MediaPlayer)
        sound.Stop()
    End Function
    Public Function PlayEnterDoor()
        enterDoor.Play()
    End Function
    Public Function PlayLeaveRoom()
        leaveRoom.Play()
    End Function
    Public Function PlayMetalDoor()
        OpenMetalDoor.Play()
    End Function
    Public Function PlayEnterDoorKnob()
        OpenDoorKnob.Play()
    End Function
    Public Function PlayCow()
        Cow.Play()
    End Function
    Public Function PlayPig()
        Pig.Play()
    End Function
    Public Function PlayChicken()
        Chicken.Play()
    End Function
    Public Function PlayEnemyAttack(monster As Character)
        Select Case monster.Name
            Case "Spider"

            Case "Skeleton"

            Case "Skeleton Archer"
                'nothing
            Case "Blue Knight"

            Case "Full Plated Knight"

            Case ""

            Case Else
                PlaySword()
        End Select
    End Function
End Module