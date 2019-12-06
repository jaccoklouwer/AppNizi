import os
import requests
import pytest
from cerberus import Validator

class Test_patients():
    def __init__(self):
        self.baseurl = os.environ["AppURL"]

    def test_A(self):
        r = requests.get(self.baseurl + "")

