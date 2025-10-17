using UnityEditor;
using UnityEngine;

public class DebugRoomGeneration
{
    [MenuItem("Tools/Debug/Test Room SetPos and SetSize")]
    static void TestRoomGeneration()
    {
        Debug.Log("========== Testing Room SetPos and SetSize ==========");
        
        // Test EntranceRoom
        EntranceRoom entrance = new EntranceRoom();
        Debug.Log($"EntranceRoom created - INITIAL: left={entrance.left}, top={entrance.top}, right={entrance.right}, bottom={entrance.bottom}");
        
        entrance.SetPos(10, 5);
        Debug.Log($"After SetPos(10,5): left={entrance.left}, top={entrance.top}, right={entrance.right}, bottom={entrance.bottom}");
        
        entrance.SetSize();
        Debug.Log($"After SetSize(): left={entrance.left}, top={entrance.top}, right={entrance.right}, bottom={entrance.bottom}");
        Debug.Log($"Final size: {entrance.Width}x{entrance.Height}");
        
        // Test ExitRoom
        Debug.Log("");
        ExitRoom exit = new ExitRoom();
        Debug.Log($"ExitRoom created - INITIAL: left={exit.left}, top={exit.top}, right={exit.right}, bottom={exit.bottom}");
        
        exit.SetPos(30, 25);
        Debug.Log($"After SetPos(30,25): left={exit.left}, top={exit.top}, right={exit.right}, bottom={exit.bottom}");
        
        exit.SetSize();
        Debug.Log($"After SetSize(): left={exit.left}, top={exit.top}, right={exit.right}, bottom={exit.bottom}");
        Debug.Log($"Final size: {exit.Width}x{exit.Height}");
        
        // Test StandardRoom
        Debug.Log("");
        StandardRoom standard = new StandardRoom(4, 8);
        Debug.Log($"StandardRoom created - INITIAL: left={standard.left}, top={standard.top}, right={standard.right}, bottom={standard.bottom}");
        
        standard.SetPos(15, 20);
        Debug.Log($"After SetPos(15,20): left={standard.left}, top={standard.top}, right={standard.right}, bottom={standard.bottom}");
        
        standard.SetSize();
        Debug.Log($"After SetSize(): left={standard.left}, top={standard.top}, right={standard.right}, bottom={standard.bottom}");
        Debug.Log($"Final size: {standard.Width}x{standard.Height}");
        
        Debug.Log("========== Test Complete ==========");
    }
}

