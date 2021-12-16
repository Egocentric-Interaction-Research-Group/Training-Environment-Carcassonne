# Training-Environment-Carcassonne
Training environment for a Carcassonne AI. The training environment contains two scenes, one for training the AI using nine separate playing fields, which is what should primarily be used. And another scene that uses only one playing field, which could be useful for getting a closer look at exactly how the training is progressing.

![This is an image](https://cdn.discordapp.com/attachments/886890473715826708/918445023253630986/unknown.png) ![This is an image](https://cdn.discordapp.com/attachments/886890473715826708/918445878803578900/unknown.png)

## Installation
### ML-Agents Toolkit
In order to use the Training Environment the user needs to download the [ML-Agents Toolkit](https://github.com/Unity-Technologies/ml-agents). This project has been tested with [release 18](https://github.com/Unity-Technologies/ml-agents/tree/release_18_branch). And it is recommended to follow their documentation in order to get it installed properly.

[ML-Agents Installation with Windows and Anaconda](https://github.com/Unity-Technologies/ml-agents/blob/release_18_branch/docs/Installation-Anaconda-Windows.md)

or [ML-Agents Installation without Anaconda](https://github.com/Unity-Technologies/ml-agents/blob/release_18_branch/docs/Installation.md)

***NOTE**: it is important that the ml-agents GitHub repository is cloned/downloaded and placed in a convenient location as it will be used during training. This folder should be kept separate from the training environment.*

### Anaconda
Another program we would recommend for using the Training Environment is [Anaconda](https://www.anaconda.com/products/individual). We used Anaconda with Python version 3.8.8. It is also possible
to run the program from most other terminals. 

### Unity 
This project has been produced and tested using Unity Version: 2020.3.19f1.
## Training
### Prerequisites
Before the Step by Step Guide is carried out make sure that the following things are done:
- Drag the Carcassonne.yaml file from the `AIConfig` folder of this project into this directory: `\ml-agents-release_18\config\ppo`
- Make sure that the AI Prefab within the Prefabs folder of this project has the Behavior Name "Carcassonne", this can be checked using the inspector in Unity. 
### Step by Step Guide
1. Open the Unity Project.
2. Launch Anaconda Prompt.
3. Type in the following command to create a virtual environment for the project:
```
conda create -n ml-agents python 3.8
```
4. Type in the following command to enter the virtual environment:
```
conda activate ml-agents
```
5. Type in the following command to cd into the ml-agents folder:
```
cd C:/USERS/YOURUSERNAME/Downloads/ml-agents-release_18
```
6. Enter the following command to initiate the training, replace NAMEOFTHERUN with your desired name for the training session:
```
mlagents-learn config/ppo/Carcassonne.yaml --run-id NAMEOFTHERUN
```
7. Open up Unity and press play to begin the training. 
8. The training will stop after the the agent has carried out the maximum number of steps defined in the .yaml file, if you wish to stop the training for any reason, simply press the play button again in Unity.

### Resuming training
If the AI has been terminated early, the training can be resumed using the following command: 
```
mlagents-learn config/ppo/Carcassonne.yaml --run-id NAMEOFTHERUN --resume
```
Where NAMEOFTHERUN = the name of the run which is to be resumed. 

If the AI has completed its max steps set in the `Carcassonne.yaml` file. This can be increased by changing the variable `max_steps:`. After this is done, the training can be resumed using the same command as if it had terminated early. 

## Results
The results from the training will be placed in the `ml-agents-release_18` in the following directory `\ml-agents-release_18\results`. After a training session has been completed/terminated a folder with the name of the run for the training session will be created. Within this folder the file of interest is the file ending in `.onnx` which is the "brain" of the AI. This file can then be placed on the AI prefab under `Model` at which point it can run without training, and without the use of a terminal.

### Running a Pre-Trained AI
When you don't want to train, but instead just run a pre-trained AI, you simply select the `.onnx` your wish to run under the `Model` component in the AI prefab. **Don't forget** to run the AI using the same observation approach and size as it was trained for. You will find pre-trained AI models under [Assets/ML-Agents/Trained AI/](https://github.com/edvinbrus1/Training-Environment-Carcassonne/tree/main/Assets/ML-Agents/Trained%20AI).
