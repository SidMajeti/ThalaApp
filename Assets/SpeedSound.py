#can use either phasevocoder, wsola, or ola

from audiotsm import wsola
from audiotsm.io.wav import WavReader, WavWriter
from audiotsm.io.stream import StreamWriter

with WavReader("/Users/saikumarmajeti/Thala/Assets/Poplar Grove Square 9 (1).wav") as reader:
    with StreamWriter(reader.channels, reader.samplerate) as writer:
        tsm = wsola(reader.channels, speed=5)
        tsm.run(reader, writer)
