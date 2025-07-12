using System;
using System.Collections.Generic;
using SplashKitSDK;

namespace Swinburne_OOP_HD
{
    public class CollisionManager
    {
        public static void CheckHazardCollisions(Character character, List<Hazards> hazards, ref bool gameOver)
        {
            foreach (Hazards hazard in hazards)
            {
                if (hazard.IsCharacterInRange(character))
                {
                    if (!hazard.CanInteract(character)) // Character is NOT immune
                    {
                        gameOver = true;
                        break; // Character died
                    }
                }
            }
        }

        public static void CheckExitDoorInteractions(Character fireboy, Character watergirl, List<ExitDoor> exitDoors, ref bool fireExitReached, ref bool waterExitReached)
        {
            foreach (ExitDoor door in exitDoors)
            {
                // Check FireBoy interactions
                if (door.Type == "fire" && door.IsCharacterInRange(fireboy))
                {
                    door.Interact(fireboy);
                    if (door.IsActivated)
                    {
                        fireExitReached = true;
                    }
                }
                else if (door.Type == "fire" && !door.IsCharacterInRange(fireboy) && door.IsActivated)
                {
                    door.Interact(fireboy); // This will deactivate the door
                    fireExitReached = false;
                }

                // Check WaterGirl interactions
                if (door.Type == "water" && door.IsCharacterInRange(watergirl))
                {
                    door.Interact(watergirl);
                    if (door.IsActivated)
                    {
                        waterExitReached = true;
                    }
                }
                else if (door.Type == "water" && !door.IsCharacterInRange(watergirl) && door.IsActivated)
                {
                    door.Interact(watergirl); // This will deactivate the door
                    waterExitReached = false;
                }
            }
        }

        public static void CheckDiamondInteraction(Character character, ref List<Diamond> diamonds) 
        {
            List<Diamond> diamondsToRemove = new List<Diamond>();
            
            foreach (Diamond diamond in diamonds) 
            {
                if (diamond.IsCharacterInRange(character)) 
                {
                    if (diamond.CanInteract(character)) 
                    {
                        diamondsToRemove.Add(diamond); // Mark the actual diamond object for removal
                    }
                }
            }

            // Remove all collected diamonds
            foreach (Diamond diamond in diamondsToRemove)
            {
                diamonds.Remove(diamond);
            }
        }

        public static void CheckPlatformInteractions(Character character, List<Platform> platforms) 
        {
            foreach (Platform platform in platforms)
            {
                Physics.FixStandingOnObject(character, platform);
                Physics.HandleObjectCollision(character, platform);
                // sticky platform, fix later
            }
        }

        public static void CheckLeverInteractions(Character character, List<Lever> levers, List<Platform> platforms) 
        {
            foreach (Lever lever in levers) 
            {
                if (lever.IsCharacterInRange(character))
                {
                    lever.Interact(character);
                }

                else if (lever.Timer.IsStarted && lever.Timer.Ticks > 10000u)
                {
                    lever.ResetTimer();
                }

                foreach (Platform platform in platforms)
                {
                    if (platform.ActivatorClass == "Lever" && platform.Type == lever.Type)
                    {
                        if (lever.IsActivated)
                        {
                            platform.IsActivated = true;
                        }
                        else
                        {
                            platform.IsActivated = false;
                        }
                    }
                }
            }
        }

        public static void CheckButtonInteractions(List<Character> characters, List<Button> buttons, List<Platform> platforms)
        {
            foreach (Button button in buttons)
            {
                bool anyCharacterInRange = false;

                foreach (Character character in characters)
                {
                    if (button.IsCharacterInRange(character))
                    {
                        anyCharacterInRange = true;
                        break;
                    }
                }

                // Update button state based on whether any character is in range
                if (anyCharacterInRange)
                {
                    button.IsActivated = true;
                }
                else
                {
                    button.IsActivated = false;
                }
            }

            // Update platform states based on button groups (only once)
            foreach (Platform platform in platforms)
            {
                if (platform.ActivatorClass == "Button")
                {
                    bool anyButtonActivated = false;

                    foreach (Button button in buttons)
                    {
                        if (platform.Type == button.Type)
                        {
                            if (button.IsActivated)
                            {
                                anyButtonActivated = true;
                                break;
                            }
                        }
                    }

                    platform.IsActivated = anyButtonActivated;
                }
            }
        }

        public static void CheckBoxInteractions() { }
    }
} 