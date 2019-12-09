--Celtic Crusader Lua Script

--LOCAL vars
state = 0


--called once when game starts up
function startup()
    WindowTitle = "Celtic Crusader"
--	LuaLoadQuests("quests.quest")
end

--called regularly every frame
function update()
    Write(480, 580, "Controls: (Space) Action, (I) Inventory, (Q) Quests")
    Write(0, 540, "Portal: " .. tostring(PortalFlag))
    Write(0, 560, "Collidable: " .. tostring(CollidableFlag))
    Write(650, 0, tostring(Health) .. "/" .. tostring(HP) .. " HP")

    text = "Current quest: " .. tostring(QuestNumber) .. ": " .. QuestSummary
    if QuestCompleteFlag == true then
        text = text .. " (COMPLETE)"
    else
        text = text .. " (incomplete)"
    end
    Write(0, 580, text)
end

