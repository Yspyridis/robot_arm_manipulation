# Robot Arm Manipulation Project

This project simulates a robot arm manipulating a cloth-like mesh, including folding and interaction. The setup includes custom scripts and shaders to attempt this. 



## Overview

- **Manual Folding**: The mesh can be manipulated manually along the X-axis, with the option to automate behavior using a script that applies physics (e.g., gravity and spring forces).
- **Cloth Simulation**: Unity's built-in cloth material cannot be dynamically modified, so custom scripts and shaders are used instead.
- **Material Shader**: A double-sided shader is necessary to prevent transparency during folds. This project uses the **Ciconia Studio shader** for this purpose.

## Scripts

- **`TouchTest.cs`**  
   - Handles the interaction between the gripper and the cloth mesh.
   - Allows manipulation of the mesh vertices where the gripper makes contact.
   - Enables interaction between the cloth and a box collider, making it useful for scenarios where the cloth needs to lie flat.

- **`CCloth.cs`**  
   - NOTE: This script can be resource-intensive, potentially causing Unity to run slowly due to the heavy rendering involved so not recomended unless simplified. 
   - Attaches to the cloth object to simulate realistic cloth behavior.
   - Adds physics properties like gravity and elasticity to the cloth mesh.
   - Requires setting up capsule colliders in the collision constraints for proper collision handling.
   

3. **`FoldMesh.cs`**  
   - Initiates cloth movement and folding automatically when ticked.
   - Useful for showing the folding process through automated triggers.


### Robot Arm Movement

- Control the robot arm using the buttons on the control panel.
- The movement is designed to be accurate but may need calibration or updates for precise real-world simulation.


## Setup Instructions

1. **Attach Scripts**:  
   - Add `TouchTest.cs` to enable vertex manipulation on contact.
   - Use `CCloth.cs` if you want the mesh to simulate more complex cloth behavior, but be aware of potential performance impacts.
   - Attach `FoldMesh.cs` if you want folding to start automatically when the gripper touches the cloth.

## To-Do List


- [ ] Integrate ML Agents to automate the folding process.
- [ ] Implement Z-axis folding.
- [ ] Review and refine the robot arm movement logic for greater accuracy.
- [ ] Trigger cloth movement with `FoldMesh` when the gripper touches the cloth.


- **Install ML Agents** (Optional):  
   - To automate the folding motion, integrate Unity's ML Agents for machine learning-based control. This can be especially useful for complex folding scenarios like folding along the Z-axis.

- The movement of the cloth needs to be kicked off by the gripper touching the cloth if using the script `FoldMesh`.
