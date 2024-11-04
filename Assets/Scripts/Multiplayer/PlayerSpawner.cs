using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using Newtonsoft.Json.Linq;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab; // Assign your player prefab in the inspector
    private SocketIOUnity socket;

    void Start()
    {
        // Initialize the Socket.IO client with the server URL (localhost)
        var uri = new System.Uri("http://localhost:11100");
        socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Query = new Dictionary<string, string>
            {
                {"token", "UNITY" }
            },
            EIO = 4,
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });

        // Connect to the server
        socket.Connect();

        // Subscribe to events
        socket.OnConnected += (sender, e) =>
        {
            Debug.Log("Connected to server");
            CreateRoom("Room1");
        };

        // Use the correct method signature for event handling
        socket.OnUnityThread("player_spawned", OnPlayerSpawned);
        socket.OnUnityThread("room_created", OnRoomCreated);
    }

    private void CreateRoom(string roomId)
    {
        var json = new JObject
        {
            ["room_id"] = roomId
        };
        socket.Emit("create_room", json);
    }

    private void OnRoomCreated(SocketIOResponse response)
    {
        var data = response.GetValue<JObject>();
        string roomId = data["room_id"].ToString();
        Debug.Log($"Room created: {roomId}");
        JoinRoom(roomId);
    }

    private void JoinRoom(string roomId)
    {
        var json = new JObject
        {
            ["room_id"] = roomId,
            ["id"] = GetInstanceID() // Use a unique ID for the player
        };
        socket.Emit("join_room", json);
    }

    // Ensure this method matches the expected signature for the event
    private void OnPlayerSpawned(SocketIOResponse response)
    {
        // Extract player data from the response
        var data = response.GetValue<JObject>();
        string playerId = data["id"].ToString();
        Vector3 position = new Vector3(
            data["position"]["x"].ToObject<float>(), 
            data["position"]["y"].ToObject<float>(), 
            data["position"]["z"].ToObject<float>()
        );

        // Instantiate the player prefab
        GameObject player = Instantiate(playerPrefab, position, Quaternion.identity);
        player.name = playerId; // Name the player for reference
        Debug.Log($"Player {playerId} spawned at {position}");
    }

    public void SpawnPlayer()
    {
        Vector3 spawnPosition = new Vector3(0, 0, 0); // Set a spawn position
        GameObject player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

        // Send spawn message to server
        var json = new JObject
        {
            ["id"] = player.GetInstanceID(), // Use the player's unique ID
            ["position"] = new JObject
            {
                ["x"] = spawnPosition.x,
                ["y"] = spawnPosition.y,
                ["z"] = spawnPosition.z
            }
        };
        socket.Emit("spawn_player", json);

        Debug.Log("Player spawned at: " + spawnPosition);
    }
}
