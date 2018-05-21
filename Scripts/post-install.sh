#!/bin/bash
apt-get -qq update
apt-get -qq --yes install python-pip python-dev build-essential python-virtualenv
pip install --upgrade virtualenv 
pip install tensorflow-gpu pandas astropy pydl --user
apt-get -qq --yes install libcupti-dev
export LD_LIBRARY_PATH=${LD_LIBRARY_PATH:+${LD_LIBRARY_PATH}:}/usr/local/cuda/extras/CUPTI/lib64
python -m pip install --user numpy scipy matplotlib ipython jupyter pandas sympy nose 
apt-get -qq --yes install pkg-config zip g++ zlib1g-dev unzip python
wget 'https://github.com/bazelbuild/bazel/releases/download/0.13.0/bazel-0.13.0-installer-linux-x86_64.sh' 
chmod +x bazel-0.13.0-installer-linux-x86_64.sh 
./bazel-0.13.0-installer-linux-x86_64.sh --user 
rm ./bazel-0.13.0-installer-linux-x86_64.sh -y 
export PATH="$PATH:$HOME/bin" 
pip install --upgrade absl-py
git clone https://github.com/tensorflow/models.git
