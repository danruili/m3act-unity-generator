# M3Act Unity Data Generator  

[![Project Page](https://img.shields.io/badge/Project-Page-green)](https://cjerry1243.github.io/M3Act/)
[![Paper](https://img.shields.io/badge/Paper-Link-blue)](https://openaccess.thecvf.com/content/CVPR2024/html/Chang_Learning_from_Synthetic_Human_Group_Activities_CVPR_2024_paper.html)
[![arXiv](https://img.shields.io/badge/arXiv-2306.16772-red)](https://arxiv.org/abs/2306.16772)  

This repository provides the Unity project, including core modules and assets, for generating synthetic data with M3Act.  

### Additional Resources  
- **[Project Page](https://cjerry1243.github.io/M3Act/):** Learn more about M3Act.  
- **[Official Repository](https://github.com/cjerry1243/M3Act):** Access M3Act3D data, group activity generation baselines, and supporting tools.  

## Requirements

To run this repository, you will need:  

- **Unity 2021.3.9f1** (Download from the [Unity Editor Archive](https://unity.com/releases/editor/archive))  

- The following Unity packages will be downloaded when building the project:  
  - **[Unity Perception](https://github.com/Unity-Technologies/com.unity.perception):** Provides dataset generation tools for computer vision.  
  - **[Unity Synthetic Humans](https://github.com/Unity-Technologies/com.unity.cv.synthetichumans):** Includes realistic 3D human models for synthetic data generation.  


## Setup
Clone the repo using the following script.
```bash
git clone https://github.com/danruili/m3act-unity-generator.git
cd m3act-unity-generator
```

Download assets from Huggingface Dataset Link:
```bash
git clone https://huggingface.co/datasets/Rosso987/M3Act-Generator-Assets
```

1. Unzip HDRI maps (`HDRI.zip`) at ```Assets/Resources/HDRI```
2. Unzip Human Avatar Part 1 (`Characters-Ready.zip`) at ```Assets/Characters/Ready```
3. Unzip Human Avatar Part 2 (`MigratedAssets-HumanModels.zip`) at ```Assets/MigratedAssets/HumanModels```
