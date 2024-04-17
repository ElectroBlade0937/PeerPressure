using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEditor.XR;
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "Collect Path", menuName = "Collect Path")]
public class CollectPath : PathObject
{

    public static int numItem = 1;
    public string[] itemlist= new string[numItem];
    public string[] itemlistNeed;
    public string[] itemlistCorrect;

    [SerializeField]
    private FriendData FriendData;

    //NPC Dialogue
    public NarrativeNode dialogueEndPath;

    public NarrativeNode dialogueClimax;

    public NarrativeNode dialogueReplaceRoot;

    public bool obtainedAll = false;
    
    public void begin(GameObject obj, Inventory pathInventory)
    {
        pathComplete = false;
        if (obj.GetComponent<Inventory>() == null)
        {
            return;
        }
        pathInventory = obj.GetComponent<Inventory>();
        itemlistNeed = new string[numItem];
        itemlistCorrect = new string[numItem];
        for(int i = 0; i  < numItem; i++)
        {
            itemlistNeed[i] = itemlist[i];
            itemlistCorrect[i] = null;
        }
        obtainedAll = false;
    }
    public void checkPath(Inventory pathInventory, Link link)
    {
        if (pathInventory == null)
        {
            Debug.Log("there not inventory");
            return;
        }
        for (int i = 0;i < pathInventory.m_Inventory.Length; i++)
        {
            update(pathInventory.m_Inventory[i]);
        }
        for (int i = 0; i < pathInventory.HotbarInventory.Length; i++)
        {
            update(pathInventory.HotbarInventory[i]);
        }
        bool sf_setpathComplete = false;
        for (int i = 0; i < numItem; i++)
        {
            if (itemlistCorrect[i] != null) {
                if (itemlistCorrect[i].Contains(itemlistNeed[i]))
                {
                    sf_setpathComplete = true;
                }
                else
                {
                    sf_setpathComplete = false;
                    break;
                }
            }
        }
        obtainedAll = sf_setpathComplete;
        UpdateDialogue(link.GetComponent<Link>().GetObject(NPC_Name).GetComponent<NPC_Movement>(), 0);
    }
    public void update(GameObject obj)
    {
        if (obj == null) { return; }
        if (obj.GetComponent<Object_Data>() != null)
        {
            Object_Data item = null;
            int index = 0;
            for (int i = 0; i < itemlistNeed.Length; i++)
            {
                if (obj.GetComponent<Object_Data>().object_name.Contains(itemlistNeed[i]))
                {
                    item = obj.GetComponent<Object_Data>();
                    index = i;
                    break;
                }
            }
            if (item != null)
            {
                itemlistCorrect[index] = item.name;
            }
        }
    }
    public void end(Link link)
    {
        FriendData.Friend += 4;
        pathComplete = true;
        pathBegin = false;
        UpdateDialogue(link.GetComponent<Link>().GetObject(NPC_Name).GetComponent<NPC_Movement>(), 1);
    }
    public void takeItem(Inventory pathInventory, Link link)
    {
        if (pathInventory == null)
        {
            Debug.Log("there not inventory");
            return;
        }
        for(int j = 0; j < numItem; j++)
        {
            for (int i = 0; i < pathInventory.m_Inventory.Length; i++)
            {
                if (pathInventory.m_Inventory[i] == null) { continue; }
                if (pathInventory.m_Inventory[i].GetComponent<Object_Data>() != null)
                {
                    Object_Data temp_object = pathInventory.m_Inventory[i].GetComponent<Object_Data>();
                    if (temp_object.object_name.Contains(itemlist[j]))
                    {
                        itemlistCorrect[j] = null;
                        pathInventory.m_Inventory_UI[i].GetComponent<RawImage>().texture = null;
                        Destroy(pathInventory.m_Inventory[i]);
                        pathInventory.m_Inventory[i] = null;
                    }
                }
            }
            
            for (int i = 0; i < pathInventory.HotbarInventory.Length; i++)
            {
                if (pathInventory.HotbarInventory[i] == null) { continue; }
                if (pathInventory.HotbarInventory[i].GetComponent<Object_Data>() != null)
                {
                    Object_Data temp_object = pathInventory.HotbarInventory[i].GetComponent<Object_Data>();
                    if (temp_object.object_name.Contains(itemlist[j]))
                    {
                        itemlistCorrect[j] = null;
                        pathInventory.HotbarInventory_UI[i].GetComponent<RawImage>().texture = null;
                        Destroy(pathInventory.HotbarInventory[i]);
                        pathInventory.HotbarInventory[i] = null;
                    }
                }
            }
        }
        end(link);
    }

    public void UpdateDialogue(NPC_Movement npc, int type)
    {
        if ((obtainedAll == true)&&(type == 0))
        {
            npc.NPC_Dialogue = dialogueEndPath;
        }
        else if ((obtainedAll == false)&&(type == 0))
        {
            npc.NPC_Dialogue = dialogueClimax;
        }
        else if ((obtainedAll == true)&&(type == 1))
        {
            npc.NPC_Dialogue = dialogueReplaceRoot;
        }
    }
}
