import os
import json
import zipfile
import urllib.request


api_url = "https://api.github.com/repos/bangumi/Archive/releases/tags/archive"

print(f"Fetching latest release")

with urllib.request.urlopen(api_url) as response:
    assert response.code == 200
    data = json.loads(response.read())

assets = data["assets"]
latest = min(assets, key=lambda x: x["updated_at"])
url = latest["browser_download_url"]

print(f"Downloading {url}")

extract_dir = "dump/"
zip_file_path = f"dump/{latest['name']}"

os.mkdir(extract_dir)

urllib.request.urlretrieve(url, zip_file_path)

print(f"Extracting {zip_file_path}")

with zipfile.ZipFile(zip_file_path, 'r') as zip_ref:
    zip_ref.extractall(extract_dir)

os.remove(zip_file_path)

print("Done")
