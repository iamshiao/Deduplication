# Deduplication
Implementation of multiple deduplication algorithms in CSharp and demo with Winform application.

## Algorithms
We have implemented the following algorithms
1. BSW - [A Low-bandwidth Network File System](https://pdos.csail.mit.edu/papers/lbfs:sosp01/lbfs.pdf)
2. TTTD - [A Framework for Analyzing and Improving Content-Based Chunking Algorithms](https://www.researchgate.net/publication/244956088_A_Framework_for_Analyzing_and_Improving_Content-Based_Chunking_Algorithms)
3. OTTTD - Improving duplicate elimination in storage systems
4. **TTTD-S** - [A Running Time Improvement for Two Thresholds Two Divisors Algorithm](https://core.ac.uk/download/pdf/70407797.pdf)

To me, the **TTTD-S thesis** is the best to begin with if you want to know the process flow of the TTTD family series. We expand the code mainly based on the procedure code in that paper.

You can find the implement in detail under `Deduplication.Controller.Algorithm`.

## UI operation
![The winform looks like](https://i.imgur.com/vN5lb1x.jpg)
1. Use Algorithm combobox too determine which deduplication algorithm to apply.
2. For Storage combobox, MemoryStorage will simple keep the `byte[]` read from file in `List<Chunk>`. On the other hand, LocalStorage will write the chunks into `$@"{Path.GetTempPath()}/chunkstore/{algorithm}"`.
3. Paste the source folder that contains the files you would like to dedu. (Or select the path by folder button)
4. Click the Run button and you will see the progress form popup that will keep on reporting you dedu percentage.

## Code
`DeduplicateController` controls the major workflow and you can adjust the alogorithm paramemters like minT, maxT, etc here too. 

By reference to `Deduplication.Controller` and `Deduplication.Model` you can transplant the dedu core of this project and operate it through `DeduplicateController` instance in your own application.
