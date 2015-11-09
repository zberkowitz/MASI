# Multi-source Ambisonic Spatialization Interface (MASI)

## What is MASI?
MASI is a series of patchers for [Cycling '74's Max](https://cycling74.com/) that provide a simplified interface for the realistic spatial positioning of sound sources in a virtual 3D environment through [ambisonic](https://en.wikipedia.org/wiki/Ambisonics) panning and virtual acoustics.  Because MASI uses ambisonic panning, the sounds can be decoded for playback on multichannel speaker arrays (e.g. 5.1, 7.1, octophonic, or any arbitrary array) or for binaural playback on headphones and standard stereo systems.

MASI does not provide a graphical panning interface itself, but instead connects to other user-created graphical interfaces through [Open Sound Control (OSC)](http://opensoundcontrol.org/introduction-osc) communication.  MASI is primarily intended to be used in conjunction with 3D game-like virtual world environments/interfaces.  Scripts are provided to connect MASI with the [Unity](https://unity3d.com/) game engine.

MASI is licensed is under the terms of the [GNU Public License](http://www.gnu.org/copyleft/gpl.html).

## Instructions

### Installation

#### Dependencies
* MASI uses CICM's [HOA Library](http://www.mshparisnord.fr/hoalibrary/en/) ([source](https://github.com/CICM/HoaLibrary-Max/)) (v. 2.2 or higher) for various ambisonic encoding and decoding tasks. Download the HOA Library for Max [here](http://www.mshparisnord.fr/hoalibrary/en/downloads/max/) and place it in your Max Packages folder.

#### Installation Instructions
* MASI is structured as a Max Package.  To install, [download the repository as a .zip](https://github.com/zberkowitz/MASI/archive/master.zip), de-compress, and place the folder in your Max Packages folder (in Max 7 this is located at `/Documents/Max 7/Packages` on both Mac and Windows).

### How to Use

#### Components
There are 3 basic components needed to use MASI:

1. Channel configurations
2. Sound sources
3. Compositions


1. A "channel configuration" is a user-supplied list of azimuth/elevation coordinates specifying the speaker setup (this is unnecessary if using binaural rendering).  This should be created as a single-line `.txt` text file.  For example, a typical 4-channel setup (Lf, Rf, Ls, Rs) would read as follows (in degrees, 0&deg; is front, direction of rotation is counterclockwise):
`45 0 315 0 135 0 225 0`.  Azimuth is between 0&deg; and 360&deg;, while elevation is between 90&deg; (directly above listener) and -90&deg; (directly below listener).  See the provided example channel configurations (under `misc/channelconfigs`) for more example configurations.

2. A "sound source" is a stream of audio in Max.  Sound sources are made available to MASI through outlets in Max abstractions.  A very simple example of a MASI sound source is the following patch, placed somewhere in the Max search path:

    ![Alt text](http://zakberkowitz.com/images/simplesource.png)

3. A "composition" is a user-supplied JSON file that contains key-value pairs denoting an abstraction with one or more sound sources (outlets) and a unique name for each source.  For example, if the simple patch shown above was saved as `source.maxpat` somewhere in the Max search path, and the single sound source it contained should be called `uniqueName`, then the JSON should read as follows:
```JSON
{
    "source" : "uniqueName"
}
```
It is also possible to have a single abstraction with multiple sources (outlets):
```JSON
{
    "source": ["uniqueName1", "uniqueName2", "uniqueName3"]
}
```
Or multiple abstractions with any combination of single or multiple sources:
```JSON
{
    "source1": "uniqueName",
    "source2": ["uniqueName1", "uniqueName2"]
}
```

With these three components in mind, Open the MASI main patch, found in the Max `Extras` menu, and follow these steps:

1. Set the decoding mode to `regular` (default) or `binaural`.
2. Load the channel configuration (single-line `.txt` file).  This step is unnecessary if using binaural mode.
3. Set the composition (JSON file).
4. Load the composition (this final step loads the abstractions specified in the composition JSON file and connects their outlets to MASI).

![Alt text](http://zakberkowitz.com/images/masimain.png)

#### Spatializing Sound Sources

##### OSC Communication

###### Moving a Sound Source
Once a composition has been loaded, the sound sources can be moved through space using OSC messages to address each source individually.  When open, MASI is receiving OSC at the local IP address on the port specified in the `OSC Receiver` portion of the main patch.  The address of each source is determined by the unique name assigned in the composition JSON file.  For example, given the following simple composition:
```JSON
{
    "source" : "uniqueName"
}
```
the single sound source can be moved by sending the OSC message `/uniqueName/position X Y Z` to the receive port, where X Y and Z are coordinates in 3D space.  

MASI uses a left-handed coordinate system with vertical Y, as shown below:  

![Alt text](http://zakberkowitz.com/images/left-handed-coordinates.png)

MASI treats a unit as 1 meter.  For example, a sound source at position `-5 0 0` will sound as though it is 5 meters to the left of a centered listener (achieved using a combination of ambisonic panning and acoustic principles such as amplitude scaling, delay, filtering, and reverb scaling).

###### Moving the Listener
Additionally, the position and rotation of the listener or "camera" in a first-person virtual world environment can be changed by sending the OSC messages `/position X Y Z` and `/rotation X Y Z`

###### Named Receives and Enable
Coming Soon.

#### Using Unity
One of the best use-cases for MASI is in conjunction with the Unity game engine and editor.  Two C# scripts for Unity are provided in the `misc/Unity` folder.  `ObjectOSC.cs` can be attached to any Unity game object and reports the position of the game object, while `CameraOSC.cs` can be attached to the main camera in a first-person setup and reports the position and rotation of the camera.  Unity games can be compiled for a variety of platforms, and Unity 5 enables easy integration with virtual reality HMDs such as the Oculus Rift.

##### Dependencies
The Unity C# scripts use Jorge Garcia's [UnityOSC](https://github.com/jorgegarcia/UnityOSC).  Download and follow the instructions for incorporating UnityOSC into your Unity project before attempting to use the MASI Unity scripts.  You will also need to follow the instructions for adding a new OSC Client to the `OSCHandler.cs` script.

##### Camera OSC
The `CameraOSC.cs` script should be attached to the main camera in a first-person environment.  The easiest way to set this up is to use the `FPSController` prefab found in the Unity Standard Assets.  `CameraOSC.cs` should be added to the `FirstPersonCharacter` prefab (which is a child of `FPSController`).  You will need to set the public variable `OSC Client Name` to the name of the OSC Client created in `OSCHandler.cs` (`Max` in the following example).  The camera will now initialize the `OSC Handler` and report its position and rotation (**Note**: since this script handles initialization, if you are **not** using the `CameraOSC.cs` script then the `OSCHandler.Instance.Init()` method will need to be called elsewhere):

![Alt text](http://zakberkowitz.com/images/cameraosc.png)

##### Object OSC
The `ObjectOSC.cs` script can be attached to any game object to report that game object's position when moved.  This script has two public variables, `Unique Name` and `OSC Client Name`.  If left blank, these variables will default to the name of the game object and the OSC Client specified by the Camera OSC script, respectively, but can be set differently if desired:

![Alt text](http://zakberkowitz.com/images/objectosc.png)

The `Unique Name` corresponds to the unique name specified in the composition JSON.  Therefore, a Unity game object named `Cube` and a composition JSON such as:
```JSON
{
    "abstractionName" : "Cube"
}
```  
will interact properly.
