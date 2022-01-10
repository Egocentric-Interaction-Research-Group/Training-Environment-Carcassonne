# Changes from cARcassonne Repo

This file will illustrate and explain the structural and logical changes made in the training environment. By reading this document, you should have
an understanding of the general funcionality of the training environment, how it differs from the cArcassonne repository and why the changes were made.

## Scripts & project structure

Every change below has either been addressed due to logic faults when applying the main repository's structure onto the new scenes and scripts we wish to utilize, or
to minimize the amount of game objects and monobehavior scripts that affect the space and load time of the game. This was important as the training process would require
simultaneous training and the game running at a higher speed than when played by a person.

### Photon Network

The photon network structure was a big roadblock to properly control the game process so that efficient training could take place. The first change we made when moving the code base from the main repository into this training environment was to get rid of any and all photon functionality. This is due to the fact that the photon network is used for network connectivity between players, not something that we had any use for when training an AI, and also because the photon framework did not allow for two players from the same client to play versus each other. Since we wanted the AI to be able to learn by playing versus itself, we had to remove code base and restructure scenes (See section below).

### Scriptable Objects

Data Containers such as scriptable objects were used in the main repository to store data such as tiles, meeples, players etc. This ment that instead of instantiate some prefab with the data, these containers would store it and be accessable anywhere in the scene. For the actual game, this was a good solution for data storage, but in the training environment developed in this repository, we felt it was necessary to maintain a multitude of scenes for different types of training and testing. One such scene is (see section below for more information) MultiBoardScene that would require us to have unique data for every board in that scene, which in turn ment that the architecture established in the main repository was not functional in our training environment. 

The solution to this issue was to remake every state class into a regular class (not monobehavior) and instead of attaching the script to an object in the scene, state would be be created at runtime inside the GameController and then distributed the other script that had previously used the scriptable objects.

### Prefabs

The main repository has a large amount of prefabs to store game data that are both visual and nonvisual. One of our goals with this training environment was to increase efficiency. In order to do that, we got rid of any and all prefabs that did not serve a purpose during training. This ment that tiles, meeples, players and even the board were no longer being represented as a prefab of some sort. Beside removing these prefabs, we have added two new ones. 

AI is a prefab that represents an AI player. This prefab is strictly nonvisual and is a required prefab for the ML-Agent framework to operate within the game scene. It is attached to the GameController and is instansiate when the scene loads. 

VisualizationBoard is a prefab that is the only visual element in the training environment. It gathers all the useful visual data that the main game used and places it into one grid object.

### Shader

The shader is a new concept that we came up with to still have a visual representation of the game state, which would still be useful for testing purposes. The shader is made up of two parts. The VisualizationBoard prefab (see section above) and the Carcassonne Visualization script. The script is fed state data via the GameController and creates a grid based on the number of tiles on the board, while still maintaining the same dimensions in the scene. This also made the game more readable when larger board states are created. 

### Scenes

When conducting AI training through reinforcement learning, playing Agents will contribute to the same training graph and learn together how to play the game. While we had
the idea that we would like at some point for the AI to play versus itself, it also became clear that we could increase training productivity by having multiple games being played at the same time. The game from the main repository did not allow this scenario, except through having differen game lobbys over the photon network. As the photon network did also not allow for multiple participants from the same client(see section above), we created a structure that allowed us to work without these restrictions.

Currently the training environment holds two different training scenes.

#### OneBoardScene

OneBoardScene is used to conduct training on a single board. This is useful to gage certain types of game loop problems and should also serve as the first scene to try out AI versus AI training. 

#### MultiBoardScene

MultiBoardScene consists of 9 boards placed in a grid that run concurrently with the same AI prefab (see section above), but each agent will operate independently while still contributing to the same training session. Games also start and finish independently on each board. While using 9 boards we have not experienced any drag on loadtimes within the game and as such this type of scene can and should probably be multiplied further to press training towards it's apex. 

